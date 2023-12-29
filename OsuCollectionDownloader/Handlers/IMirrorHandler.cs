using OsuCollectionDownloader.Objects;

namespace OsuCollectionDownloader.Handlers;

internal interface IMirrorHandler
{
    Task<Result<bool>> DownloadAsync(string filePath, int beatmapSetId, CancellationToken token);
}
