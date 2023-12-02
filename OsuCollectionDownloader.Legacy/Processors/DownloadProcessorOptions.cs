namespace OsuCollectionDownloader.Legacy.Processors;

internal sealed record DownloadProcessorOptions(int Id, DirectoryInfo OsuSongsDirectory, DirectoryInfo? OsdbFileDirectory);
