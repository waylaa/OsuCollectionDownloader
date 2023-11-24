using OsuCollectionDownloader.Objects;

namespace OsuCollectionDownloader.Handlers;

internal interface IBeatmapMirrorHandler
{
    Task<Result<bool>> HandleAsync(string title, string difficultyName, string filePath, CancellationToken token);
}
