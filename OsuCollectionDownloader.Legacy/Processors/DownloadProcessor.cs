using Microsoft.Extensions.Logging;
using OsuCollectionDownloader.Legacy.Extensions;
using OsuCollectionDownloader.Legacy.Logging;
using OsuCollectionDownloader.Legacy.Objects;
using System.Collections.Immutable;
using System.IO.Compression;
using System.Net.Http.Json;

namespace OsuCollectionDownloader.Legacy.Processors;

internal sealed class DownloadProcessor
(
    DownloadProcessorOptions options,
    IHttpClientFactory httpClientFactory,
    ILogger<DownloadProcessor> logger
)
{
    private static readonly ImmutableArray<string> s_beatmapMirrors =
    [
        "https://api.osu.direct/d",
        "https://api.nerinyan.moe/d",
	    "https://api.chimu.moe/download"
    ];

    private const string OsuCollectorApiBaseUrl = "https://osucollector.com/api";

    private readonly HttpClient _client = httpClientFactory.CreateClient();

    internal async Task DownloadAsync(CancellationToken ct)
    {
        logger.OngoingCollectionFetch();
        FetchedCollectionMetadata? metadata = await GetMetadataAsync(options.Id, ct);

        if (metadata is null)
        {
            logger.UnsuccessfulCollectionFetch();
            return;
        }

        List<Beatmap> downloadedBeatmaps = [];

        foreach (Beatmap beatmap in await GetBeatmapsAsync(options.Id, ct))
        {
            string beatmapFileName = $"{beatmap.BeatmapsetId} {beatmap.Beatmapset.Artist} - {beatmap.Beatmapset.Title}.osz";
            string beatmapFilePath = Path.Combine(options.OsuSongsDirectory.FullName, beatmapFileName.ReplaceInvalidPathChars());
            string beatmapInSongsDirectory = Path.Combine(options.OsuSongsDirectory.FullName, Path.GetFileNameWithoutExtension(beatmapFileName.ReplaceInvalidPathChars()));

            if (Directory.Exists(beatmapInSongsDirectory))
            {
                downloadedBeatmaps.Add(beatmap);
                logger.AlreadyExists(Path.GetFileNameWithoutExtension(beatmapFileName));

                continue;
            }

            HttpResponseMessage? response = null;

            foreach (string mirror in s_beatmapMirrors)
            {
                try
                {
                    response = await _client.GetAsync($"{mirror}/{beatmap.BeatmapsetId}", HttpCompletionOption.ResponseHeadersRead, ct);
                    break;
                }
                catch (HttpRequestException)
                {
                    continue;
                }
            }

            if (response is null || !response.IsSuccessStatusCode)
            {
                logger.UnsuccessfulDownload(Path.GetFileNameWithoutExtension(beatmapFileName));
                continue;
            }
            
            await using (FileStream beatmapStream = File.OpenWrite(beatmapFilePath))
            {
                await response.Content.CopyToAsync(beatmapStream, ct);
            }

            response.Dispose();

            if (!Extract(beatmapFilePath))
            {
                logger.ExtractionFailure(Path.GetFileNameWithoutExtension(beatmapFileName));
                File.Delete(beatmapFilePath);

                continue;
            }

            File.Delete(beatmapFilePath);
            downloadedBeatmaps.Add(beatmap);
            
            logger.SuccessfulDownload(Path.GetFileNameWithoutExtension(beatmapFileName));
        }

        logger.DownloadFinished();

        if (options.OsdbFileDirectory is not null)
        {
            await GenerateOsdbCollectionAsync(metadata, [.. downloadedBeatmaps]);
            logger.OsdbCollectionSuccessfulGeneration(options.OsdbFileDirectory!.FullName);
        }
    }

    private async Task<FetchedCollectionMetadata?> GetMetadataAsync(int id, CancellationToken ct)
    {
        try
        {
            return await _client.GetFromJsonAsync<FetchedCollectionMetadata>($"{OsuCollectorApiBaseUrl}/collections/{id}", ct);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    private async Task<ImmutableList<Beatmap>> GetBeatmapsAsync(int id, CancellationToken ct)
    {
        List<Beatmap> beatmaps = [];

        FetchedCollection? collection = await _client.GetFromJsonAsync<FetchedCollection>
        (
            $"{OsuCollectorApiBaseUrl}/collections/{id}/beatmapsv2?cursor=0&perPage=100",
            ct
        );

        if (collection is null)
        {
            return [];
        }

        do
        {
            beatmaps.AddRange(collection.Beatmaps);

            collection = await _client.GetFromJsonAsync<FetchedCollection>
            (
                $"{OsuCollectorApiBaseUrl}/collections/{id}/beatmapsv2?cursor={collection.NextPageCursor}&perPage=100",
                ct
            );

            if (collection is null)
            {
                return [];
            }

            beatmaps.AddRange(collection.Beatmaps);

            if (collection is null)
            {
                return [];
            }

        } while (!ct.IsCancellationRequested || (collection.HasMore && collection.NextPageCursor is not null));

        return [.. beatmaps];
    }

    private bool Extract(string sourceBeatmapFilePath)
    {
        try
        {
            ZipFile.ExtractToDirectory
            (
                sourceBeatmapFilePath,
                Path.Combine(options.OsuSongsDirectory.FullName, Path.GetFileNameWithoutExtension(sourceBeatmapFilePath)),
                overwriteFiles: true
            );

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async ValueTask GenerateOsdbCollectionAsync(FetchedCollectionMetadata metadata, ImmutableArray<Beatmap> downloadedBeatmaps)
    {
        string osdbFilePath = Path.Combine
        (
            options.OsdbFileDirectory!.FullName,
            $"{metadata.Name.ReplaceInvalidPathChars()}.osdb"
        );

        await using FileStream osdbFileStream = File.OpenWrite(osdbFilePath);
        await using BinaryWriter osdbWriter = new(osdbFileStream);

        osdbWriter.Write("o!dm6");
        osdbWriter.Write(DateTime.Now.ToOADate());
        osdbWriter.Write(metadata.Uploader.Username);
        osdbWriter.Write(1); // Number of collections.
        osdbWriter.Write(metadata.Name);
        osdbWriter.Write(downloadedBeatmaps.Length);

        foreach (Beatmap beatmap in downloadedBeatmaps)
        {
            osdbWriter.Write(beatmap.Id);
            osdbWriter.Write(beatmap.BeatmapsetId);
            osdbWriter.Write(beatmap.Beatmapset.Artist);
            osdbWriter.Write(beatmap.Beatmapset.Title);
            osdbWriter.Write(beatmap.Version); // Difficulty name.
            osdbWriter.Write(beatmap.Checksum);
            osdbWriter.Write(string.Empty); // Beatmapset's user comments.
            osdbWriter.Write((byte)beatmap.ModeInt);
            osdbWriter.Write(beatmap.DifficultyRating);
        }

        osdbWriter.Write(0); // Beatmaps that contain hashes only.
        osdbWriter.Write("By Piotrekol");
    }
}
