using Microsoft.Extensions.Logging;
using OsuCollectionDownloader.Core.Extensions;
using OsuCollectionDownloader.Handlers;
using OsuCollectionDownloader.Handlers.Chains;
using OsuCollectionDownloader.Logging;
using OsuCollectionDownloader.Objects;
using OsuCollectionDownloader.Services;
using System.Collections.Immutable;
using System.IO.Compression;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace OsuCollectionDownloader.Processors;

internal sealed class DownloadProcessor(DownloadProcessorOptions options, IHttpClientFactory httpClientFactory, ILogger<DownloadProcessor> logger)
{
    private const string OsuCollectorApiUrl = "https://osucollector.com/api";

    private readonly HttpClient _client = httpClientFactory.CreateClient();

    internal async Task DownloadAsync(CancellationToken ct)
    {
        if (!options.OsuSongsDirectory.FullName.Contains(Path.Combine("osu!", "Songs")))
        {
            logger.UnsupportedExtractionDirectory(options.OsuSongsDirectory.FullName);
        }

        logger.OngoingCollectionFetch();
        Result<JsonDocument?> metadataResult = await GetMetadataAsync(options.Id, ct);

        if (!metadataResult.IsSuccessful || !metadataResult.HasValue)
        {
            logger.UnsuccessfulCollectionFetch();
            return;
        }

        Result<ImmutableList<JsonElement>> fetchedBeatmapsResult = await GetBeatmapsAsync(options.Id, ct);

        if (!fetchedBeatmapsResult.IsSuccessful || !fetchedBeatmapsResult.HasValue)
        {
            logger.UnsuccessfulCollectionFetch();
            return;
        }

        MirrorChain mirrorChain =
        [
            new NerinyanMirrorHandler(new NerinyanMirrorService(_client)),
            new ChimuMirrorHandler(new ChimuService(_client)),
            new OsuDirectMirrorHandler(new OsuDirectService(_client)),
        ];

        List<JsonElement> downloadedBeatmaps = [];
        
        foreach (JsonElement beatmap in fetchedBeatmapsResult.Value!)
        {
            string beatmapFileName = $"{beatmap.GetProperty("beatmapset_id").GetInt32()} " +
                $"{beatmap.GetProperty("beatmapset").GetProperty("artist").GetString()} - {beatmap.GetProperty("beatmapset").GetProperty("title").GetString()}.osz";

            string beatmapFilePath = Path.Combine(options.OsuSongsDirectory.FullName, beatmapFileName.ReplaceInvalidPathChars());
            string beatmapInSongsDirectory = Path.Combine(options.OsuSongsDirectory.FullName, Path.GetFileNameWithoutExtension(beatmapFileName.ReplaceInvalidPathChars()));

            if (beatmapInSongsDirectory.Contains(Path.Combine("osu!", "Songs")) && Directory.Exists(beatmapInSongsDirectory))
            {
                downloadedBeatmaps.Add(beatmap);
                logger.AlreadyExists(Path.GetFileNameWithoutExtension(beatmapFileName));

                continue;
            }

            Result<bool> downloadResult = await mirrorChain.HandleAsync
            (
                beatmap.GetProperty("beatmapset").GetProperty("title").GetString()!,
                beatmap.GetProperty("version").GetString()!,
                beatmapFilePath,
                ct
            );

            if (!downloadResult.IsSuccessful || !downloadResult.Value)
            {
                logger.UnsuccessfulDownload(Path.GetFileNameWithoutExtension(beatmapFileName));
                continue;
            }

            Result<bool> extractionResult = Extract(beatmapFilePath);
            File.Delete(beatmapFilePath);

            if (!extractionResult.IsSuccessful || !extractionResult.Value)
            {
                logger.ExtractionFailure(Path.GetFileNameWithoutExtension(beatmapFileName));
                continue;
            }

            downloadedBeatmaps.Add(beatmap);
            logger.SuccessfulDownload(Path.GetFileNameWithoutExtension(beatmapFileName));
        }

        logger.DownloadFinished();

        if (options.OsdbFileDirectory is not null)
        {
            await GenerateOsdbCollectionAsync(metadataResult.Value!.RootElement, [.. downloadedBeatmaps]);
            logger.OsdbCollectionSuccessfulGeneration(options.OsdbFileDirectory!.FullName);
        }

        metadataResult.Value?.Dispose();
    }

    private async Task<Result<JsonDocument?>> GetMetadataAsync(int id, CancellationToken ct)
    {
        try
        {
            return await _client.GetFromJsonAsync<JsonDocument>($"{OsuCollectorApiUrl}/collections/{id}", ct);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private async Task<Result<ImmutableList<JsonElement>>> GetBeatmapsAsync(int id, CancellationToken ct)
    {
        try
        {
            List<JsonElement> beatmaps = [];
            JsonDocument? collection = await _client.GetFromJsonAsync<JsonDocument>($"{OsuCollectorApiUrl}/collections/{id}/beatmapsv2?cursor=0&perPage=100", ct);

            if (collection is null)
            {
                return ImmutableList<JsonElement>.Empty;
            }

            do
            {
                beatmaps.AddRange(collection.RootElement.GetProperty("beatmaps").EnumerateArray());

                collection = await _client.GetFromJsonAsync<JsonDocument>
                (
                    $"{OsuCollectorApiUrl}/collections/{id}/beatmapsv2?cursor={collection.RootElement.GetProperty("nextPageCursor").GetInt32()}&perPage=100",
                    ct
                );

                if (collection is null)
                {
                    return ImmutableList<JsonElement>.Empty;
                }

            } while (collection.RootElement.GetProperty("hasMore").GetBoolean() == true);

            collection.Dispose();
            return beatmaps.ToImmutableList();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private Result<bool> Extract(string sourceBeatmapFilePath)
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
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async ValueTask GenerateOsdbCollectionAsync(JsonElement metadata, ImmutableList<JsonElement> downloadedBeatmaps)
    {
        string osdbFilePath = Path.Combine
        (
            options.OsdbFileDirectory!.FullName,
            $"{metadata.GetProperty("name").GetString()!.ReplaceInvalidPathChars()}.osdb"
        );

        await using FileStream osdbFileStream = File.OpenWrite(osdbFilePath);
        await using (BinaryWriter osdbVersionWriter = new(osdbFileStream, Encoding.UTF8, leaveOpen: true))
        {
            osdbVersionWriter.Write("o!dm8");
        }

        await using GZipStream compressionStream = new(osdbFileStream, CompressionMode.Compress);
        await using BinaryWriter osdbWriter = new(compressionStream);

        osdbWriter.Write("o!dm8");
        osdbWriter.Write(DateTime.Now.ToOADate());
        osdbWriter.Write(metadata.GetProperty("uploader").GetProperty("username").GetString()!);
        osdbWriter.Write(1); // Number of collections.
        osdbWriter.Write(metadata.GetProperty("name").GetString()!);
        osdbWriter.Write(0); // OsuStats online id.
        osdbWriter.Write(downloadedBeatmaps.Count);

        foreach (JsonElement beatmap in downloadedBeatmaps)
        {
            osdbWriter.Write(beatmap.GetProperty("id").GetInt32());
            osdbWriter.Write(beatmap.GetProperty("beatmapset_id").GetInt32());
            osdbWriter.Write(beatmap.GetProperty("beatmapset").GetProperty("artist").GetString()!);
            osdbWriter.Write(beatmap.GetProperty("beatmapset").GetProperty("title").GetString()!);
            osdbWriter.Write(beatmap.GetProperty("version").GetString()!); // Difficulty name.
            osdbWriter.Write(beatmap.GetProperty("checksum").GetString()!);
            osdbWriter.Write(string.Empty); // Beatmapset's user comments.
            osdbWriter.Write((byte)beatmap.GetProperty("mode_int").GetInt32());
            osdbWriter.Write(beatmap.GetProperty("difficulty_rating").GetDouble());
        }

        osdbWriter.Write(0); // Beatmaps that contain hashes only.
        osdbWriter.Write("By Piotrekol");
    }
}
