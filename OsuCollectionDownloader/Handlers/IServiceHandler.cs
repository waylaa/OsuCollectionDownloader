using OsuCollectionDownloader.Objects;

namespace OsuCollectionDownloader.Handlers;

internal interface IServiceHandler
{
    Task<Result<bool>> HandleAsync(string title, string difficultyName, string filePath, CancellationToken token);
}
