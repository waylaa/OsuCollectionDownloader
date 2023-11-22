using Microsoft.Extensions.Logging;

namespace OsuCollectionDownloader.Logging;

internal static partial class DownloadProcessorLogger
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Attempting to fetch the metadata and beatmaps from the collection. This may take a while.")]
    public static partial void OngoingCollectionFetch(this ILogger logger);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Could not fetch the metadata or beatmaps from the collection.")]
    public static partial void UnsuccessfulCollectionFetch(this ILogger logger);

    [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "'{BeatmapName} already exists.'")]
    public static partial void AlreadyExists(this ILogger logger, string beatmapName);

    [LoggerMessage(EventId = 5, Level = LogLevel.Error, Message = "'{BeatmapName}' could not be extracted and has been deleted as such.")]
    public static partial void ExtractionFailure(this ILogger logger, string beatmapName);

    [LoggerMessage(EventId = 7, Level = LogLevel.Information, Message = "'{BeatmapName}' has been downloaded successfully.")]
    public static partial void SuccessfulDownload(this ILogger logger, string beatmapName);

    [LoggerMessage(EventId = 8, Level = LogLevel.Error, Message = "Could not download '{BeatmapName}'.")]
    public static partial void UnsuccessfulDownload(this ILogger logger, string beatmapName);

    [LoggerMessage(EventId = 9, Level = LogLevel.Information, Message = "Download finished.")]
    public static partial void DownloadFinished(this ILogger logger);

    [LoggerMessage(EventId = 10, Level = LogLevel.Information, Message = "Generated a .osdb collection at {OsdbFilePath}.")]
    public static partial void OsdbCollectionSuccessfulGeneration(this ILogger logger, string osdbFilePath);
}
