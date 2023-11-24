using Microsoft.Extensions.Logging;
using OsuCollectionDownloader.Extensions;
using OsuCollectionDownloader.Handlers;
using OsuCollectionDownloader.Handlers.Chains;
using OsuCollectionDownloader.Json.Contexts;
using OsuCollectionDownloader.Json.Models;
using OsuCollectionDownloader.Logging;
using OsuCollectionDownloader.Objects;
using OsuCollectionDownloader.Services;
using System.Collections.Immutable;
using System.IO.Compression;
using System.Net.Http.Json;
using System.Text;

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
        Result<FetchedCollectionMetadata?> metadataResult = await GetMetadataAsync(options.Id, ct);

        if (!metadataResult.IsSuccessful || !metadataResult.HasValue)
        {
            logger.UnsuccessfulCollectionFetch();
            return;
        }

        Result<ImmutableList<Beatmap>> fetchedBeatmapsResult = await GetBeatmapsAsync(options.Id, ct);

        if (!fetchedBeatmapsResult.IsSuccessful || !fetchedBeatmapsResult.HasValue || fetchedBeatmapsResult.Value.IsEmpty)
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

        List<Beatmap> downloadedBeatmaps = [];
        
        foreach (Beatmap beatmap in fetchedBeatmapsResult.Value)
        {
            string beatmapFileName = $"{beatmap.BeatmapsetId} {beatmap.Beatmapset.Artist} - {beatmap.Beatmapset.Title}.osz";
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
                beatmap.Beatmapset.Title,
                beatmap.Version,
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
            await GenerateOsdbCollectionAsync(metadataResult.Value!, [.. downloadedBeatmaps]);
            logger.OsdbCollectionSuccessfulGeneration(options.OsdbFileDirectory!.FullName);
        }
    }

    private async Task<Result<FetchedCollectionMetadata?>> GetMetadataAsync(int id, CancellationToken ct)
    {
        try
        {
            return await _client.GetFromJsonAsync
            (
                $"{OsuCollectorApiUrl}/collections/{id}",
                FetchedCollectionMetadataSerializationContext.Default.FetchedCollectionMetadata,
                ct
            );
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private async Task<Result<ImmutableList<Beatmap>>> GetBeatmapsAsync(int id, CancellationToken ct)
    {
        try
        {
            List<Beatmap> beatmaps = [];

            FetchedCollection? collection = await _client.GetFromJsonAsync
            (
                $"{OsuCollectorApiUrl}/collections/{id}/beatmapsv2?cursor=0&perPage=100",
                FetchedCollectionSerializationContext.Default.FetchedCollection,
                ct
            );

            if (collection is null)
            {
                return ImmutableList<Beatmap>.Empty;
            }

            do
            {
                beatmaps.AddRange(collection.Beatmaps);

                collection = await _client.GetFromJsonAsync
                (
                    $"{OsuCollectorApiUrl}/collections/{id}/beatmapsv2?cursor={collection.NextPageCursor}&perPage=100",
                    FetchedCollectionSerializationContext.Default.FetchedCollection,
                    ct
                );

                if (collection is null)
                {
                    return ImmutableList<Beatmap>.Empty;
                }

            } while (collection.HasMore);

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

    public async ValueTask GenerateOsdbCollectionAsync(FetchedCollectionMetadata metadata, ImmutableList<Beatmap> downloadedBeatmaps)
    {
        string osdbFilePath = Path.Combine(options.OsdbFileDirectory!.FullName, $"{metadata.Name.ReplaceInvalidPathChars()}.osdb");

        await using FileStream osdbFileStream = File.OpenWrite(osdbFilePath);
        await using (BinaryWriter osdbVersionWriter = new(osdbFileStream, Encoding.UTF8, leaveOpen: true))
        {
            osdbVersionWriter.Write("o!dm8");
        }

        await using GZipStream compressionStream = new(osdbFileStream, CompressionMode.Compress);
        await using BinaryWriter osdbWriter = new(compressionStream);

        osdbWriter.Write("o!dm8");
        osdbWriter.Write(DateTime.Now.ToOADate());
        osdbWriter.Write(metadata.Uploader.Username);
        osdbWriter.Write(1); // Number of collections.
        osdbWriter.Write(metadata.Name);
        osdbWriter.Write(0); // OsuStats online id.
        osdbWriter.Write(downloadedBeatmaps.Count);

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
