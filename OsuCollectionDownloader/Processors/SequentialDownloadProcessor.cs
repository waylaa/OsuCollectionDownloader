using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OsuCollectionDownloader.Cache;
using OsuCollectionDownloader.Extensions;
using OsuCollectionDownloader.Handlers;
using OsuCollectionDownloader.Handlers.Chains;
using OsuCollectionDownloader.Json.Models;
using OsuCollectionDownloader.Logging;
using OsuCollectionDownloader.Objects;
using System.Collections.Immutable;

namespace OsuCollectionDownloader.Processors;

internal sealed class SequentialDownloadProcessor(
    DownloadProcessorBaseOptions options,
    IHttpClientFactory httpFactory,
    ILogger<SequentialDownloadProcessor> logger) : DownloadProcessorBase(options, httpFactory)
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

        FrozenDirectoryCache cache = new(Options.ExtractionDirectory);

        MirrorChain mirrorChain =
        [
            new NerinyanHandler(Client),
            new ChimuHandler(Client),
            new OsuDirectHandler(Client),
        ];

        List<Beatmap> downloadedBeatmaps = [];

        foreach (Beatmap beatmap in beatmapsResult.Value)
        {
            string beatmapFileName = $"{beatmap.BeatmapsetId} {beatmap.Beatmapset.Artist} - {beatmap.Beatmapset.Title}.osz";
            string beatmapFilePath = Path.Combine(Options.ExtractionDirectory, beatmapFileName.ReplaceInvalidPathChars());
            string beatmapDirectory = Path.Combine(Options.ExtractionDirectory, Path.GetFileNameWithoutExtension(beatmapFileName.ReplaceInvalidPathChars()));

            if (cache.Items.Contains(beatmapDirectory))
            {
                downloadedBeatmaps.Add(beatmap);
                logger.AlreadyExists(Path.GetFileNameWithoutExtension(beatmapFileName));

                continue;
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
                continue;
            }

            Result<bool> extractionResult = Extract(beatmapFilePath);
            File.Delete(beatmapFilePath);

            if (!extractionResult.IsSucessfulWithValue || !extractionResult.Value)
            {
                logger.ExtractionFailure(Path.GetFileNameWithoutExtension(beatmapFileName));
                continue;
            }

            downloadedBeatmaps.Add(beatmap);
            logger.SuccessfulDownload(Path.GetFileNameWithoutExtension(beatmapFileName));
        }

        logger.DownloadFinished();

        if (Options.OsdbGenerationDirectory is not null)
        {
            await GenerateOsdbCollectionAsync(metadataResult.Value!, [.. downloadedBeatmaps]);
            logger.OsdbCollectionSuccessfulGeneration(Options.OsdbGenerationDirectory);
        }
    }
}
