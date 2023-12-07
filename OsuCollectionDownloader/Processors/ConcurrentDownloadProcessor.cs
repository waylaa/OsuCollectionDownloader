﻿using Microsoft.Extensions.Logging;
using OsuCollectionDownloader.Cache;
using OsuCollectionDownloader.Extensions;
using OsuCollectionDownloader.Handlers.Chains;
using OsuCollectionDownloader.Handlers;
using OsuCollectionDownloader.Json.Models;
using OsuCollectionDownloader.Logging;
using OsuCollectionDownloader.Objects;
using System.Collections.Immutable;

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
        if (!options.ExtractionDirectory.Contains(Path.Combine("osu!", "Songs")))
        {
            logger.UnsupportedExtractionDirectory(options.ExtractionDirectory);
        }

        logger.OngoingCollectionFetch();

        Result<FetchedCollectionMetadata?> metadataResult = await GetMetadataAsync(options.Id, token);
        if (!metadataResult.IsSucessfulWithValue)
        {
            logger.UnsuccessfulCollectionFetch();
            return;
        }

        Result<ImmutableList<Beatmap>> beatmapsResult = await GetBeatmapsAsync(options.Id, token);
        if (!beatmapsResult.IsSucessfulWithValue)
        {
            logger.UnsuccessfulCollectionFetch();
            return;
        }

        FrozenDirectoryCache cache = new(options.ExtractionDirectory);

        MirrorChain mirrorChain =
        [
            new NerinyanHandler(_client),
            new ChimuHandler(_client),
            new OsuDirectHandler(_client),
        ];

        List<Beatmap> downloadedBeatmaps = [];

        using (SemaphoreSlim concurrencyLimiter = new(initialCount: 2))
        {
            ImmutableArray<Task> downloads = beatmapsResult.Value.Select(async beatmap =>
            {
                await concurrencyLimiter.WaitAsync(token);

                string beatmapFileName = $"{beatmap.BeatmapsetId} {beatmap.Beatmapset.Artist} - {beatmap.Beatmapset.Title}.osz";
                string beatmapFilePath = Path.Combine(options.ExtractionDirectory, beatmapFileName.ReplaceInvalidPathChars());
                string beatmapDirectory = Path.Combine(options.ExtractionDirectory, Path.GetFileNameWithoutExtension(beatmapFileName.ReplaceInvalidPathChars()));

                if (cache.Items.Contains(beatmapDirectory))
                {
                    downloadedBeatmaps.Add(beatmap);
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

                downloadedBeatmaps.Add(beatmap);
                logger.SuccessfulDownload(Path.GetFileNameWithoutExtension(beatmapFileName));

                concurrencyLimiter.Release();

            }).ToImmutableArray();

            await Task.WhenAll(downloads);
        }

        logger.DownloadFinished();

        if (options.OsdbGenerationDirectory is not null)
        {
            await GenerateOsdbCollectionAsync(metadataResult.Value!, [.. downloadedBeatmaps]);
            logger.OsdbCollectionSuccessfulGeneration(options.OsdbGenerationDirectory);
        }
    }
}
