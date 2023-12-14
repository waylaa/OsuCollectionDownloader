using Microsoft.Extensions.Logging;
using OsuCollectionDownloader.Extensions;
using OsuCollectionDownloader.Handlers.Chains;
using OsuCollectionDownloader.Handlers;
using OsuCollectionDownloader.Json.Models;
using OsuCollectionDownloader.Logging;
using OsuCollectionDownloader.Objects;
using System.Collections.Immutable;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Frozen;
using Microsoft.Extensions.DependencyInjection;

namespace OsuCollectionDownloader.Processors;

internal sealed class ConcurrentDownloadProcessor
(
    DownloadProcessorBaseOptions options,
    IHttpClientFactory clientFactory,
    ILogger<ConcurrentDownloadProcessor> logger
) : DownloadProcessorBase(options, clientFactory)
{
    internal override async Task DownloadAsync(CancellationToken token)
    {
        if (!Options.ExtractionDirectory.Contains(Path.Combine("osu!", "Songs")))
        {
            logger.UnsupportedExtractionDirectory(Options.ExtractionDirectory);
        }

        logger.OngoingCollectionFetch();

        Result<FetchedCollectionMetadata?> metadataResult = await GetMetadataAsync(token);
        if (!metadataResult.IsSucessfulWithValue)
        {
            logger.UnsuccessfulCollectionFetch();
            return;
        }

        Result<ImmutableList<Beatmap>> beatmapsResult = await GetBeatmapsAsync(token);
        if (!beatmapsResult.IsSucessfulWithValue)
        {
            logger.UnsuccessfulCollectionFetch();
            return;
        }

        List<Beatmap> downloadedBeatmapsets = [];

        using (IMemoryCache directoryCache = Program.Services.GetRequiredService<IMemoryCache>())
        {
            FrozenSet<string> cache = directoryCache.Set("Songs", new HashSet<string>(Directory.EnumerateDirectories(Options.ExtractionDirectory), StringComparer.OrdinalIgnoreCase).ToFrozenSet());

            MirrorChain mirrorChain =
            [
                new NerinyanHandler(Client),
                new ChimuHandler(Client),
                new OsuDirectHandler(Client),
            ];

            using SemaphoreSlim concurrencyLimiter = new(initialCount: 2);

            ImmutableArray<Task> downloads = beatmapsResult.Value.DistinctBy(x => x.BeatmapsetId).Select(async beatmap =>
            {
                await concurrencyLimiter.WaitAsync(token);

                string beatmapFileName = $"{beatmap.BeatmapsetId} {beatmap.Beatmapset.Artist} - {beatmap.Beatmapset.Title}.osz";
                string beatmapFilePath = Path.Combine(Options.ExtractionDirectory, beatmapFileName.ReplaceInvalidPathChars());
                string beatmapDirectory = Path.Combine(Options.ExtractionDirectory, Path.GetFileNameWithoutExtension(beatmapFileName.ReplaceInvalidPathChars()));

                if (cache.Contains(beatmapDirectory))
                {
                    downloadedBeatmapsets.Add(beatmap);
                    logger.AlreadyExists(Path.GetFileNameWithoutExtension(beatmapFileName));

                    concurrencyLimiter.Release();
                    return;
                }

                Result<bool> downloadResult = await mirrorChain.HandleAsync
                (
                    beatmapFilePath,
                    beatmap.BeatmapsetId,
                    token
                );

                if (!downloadResult.IsSucessfulWithValue || !downloadResult.Value)
                {
                    logger.UnsuccessfulDownload(Path.GetFileNameWithoutExtension(beatmapFileName));

                    concurrencyLimiter.Release();
                    return;
                }

                Result<bool> extractionResult = Extract(beatmapFilePath);
                File.Delete(beatmapFilePath);

                if (!extractionResult.IsSucessfulWithValue || !extractionResult.Value)
                {
                    logger.ExtractionFailure(Path.GetFileNameWithoutExtension(beatmapFileName));

                    concurrencyLimiter.Release();
                    return;
                }

                downloadedBeatmapsets.Add(beatmap);
                logger.SuccessfulDownload(Path.GetFileNameWithoutExtension(beatmapFileName));

                concurrencyLimiter.Release();

            }).ToImmutableArray();

            await Task.WhenAll(downloads);
        }

        logger.DownloadFinished();

        if (Options.OsdbGenerationDirectory is not null)
        {
            await GenerateOsdbCollectionAsync(metadataResult.Value, beatmapsResult.Value.UnionBy(downloadedBeatmapsets, x => x.Version).ToImmutableList());
            logger.OsdbCollectionSuccessfulGeneration(Options.OsdbGenerationDirectory);
        }
    }
}
