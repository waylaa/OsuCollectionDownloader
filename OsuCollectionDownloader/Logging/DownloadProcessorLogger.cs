using Microsoft.Extensions.Logging;

namespace OsuCollectionDownloader.Logging;

internal static partial class DownloadProcessorLogger
{
    [LoggerMessage
    (
        EventId = 0,
        Level = LogLevel.Warning,
        Message = "The specified directory '{extractionDirectory}' does not point to the 'Songs' folder within your osu! directory. " +
        "Failing to do so prevents the application from checking for existing beatmaps, potentially slowing down the download progress " +
        "and extracted beatmaps will not be able to be saved in the 'Songs' folder."
    )]
    internal static partial void UnsupportedExtractionDirectory(this ILogger logger, string extractionDirectory);

    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Attempting to fetch the metadata and beatmaps from the collection. This may take a while.")]
    internal static partial void OngoingCollectionFetch(this ILogger logger);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Could not fetch the metadata or beatmaps from the collection.")]
    internal static partial void UnsuccessfulCollectionFetch(this ILogger logger);

    [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "'{BeatmapName}' already exists.")]
    internal static partial void AlreadyExists(this ILogger logger, string beatmapName);

    [LoggerMessage(EventId = 5, Level = LogLevel.Error, Message = "'{BeatmapName}' could not be extracted and has been deleted as such.")]
    internal static partial void ExtractionFailure(this ILogger logger, string beatmapName);

    [LoggerMessage(EventId = 7, Level = LogLevel.Information, Message = "'{BeatmapName}' has been downloaded successfully.")]
    internal static partial void SuccessfulDownload(this ILogger logger, string beatmapName);

    [LoggerMessage(EventId = 8, Level = LogLevel.Error, Message = "Could not download '{BeatmapName}'.")]
    internal static partial void UnsuccessfulDownload(this ILogger logger, string beatmapName);

    [LoggerMessage(EventId = 9, Level = LogLevel.Information, Message = "Download finished.")]
    internal static partial void DownloadFinished(this ILogger logger);

    [LoggerMessage(EventId = 10, Level = LogLevel.Information, Message = "Generated a .osdb collection at {OsdbFilePath}.")]
    internal static partial void OsdbCollectionSuccessfulGeneration(this ILogger logger, string osdbFilePath);
}
