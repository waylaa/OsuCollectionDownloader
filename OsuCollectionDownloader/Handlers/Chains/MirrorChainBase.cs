using OsuCollectionDownloader.Objects;

namespace OsuCollectionDownloader.Handlers.Chains;

internal abstract class MirrorChainBase : IMirrorHandler
{
    protected abstract ISet<IMirrorHandler> Mirrors { get; }

    public abstract Task<Result<bool>> HandleAsync(string title, string difficultyName, string filePath, CancellationToken token);

    protected internal abstract MirrorChainBase Add(IMirrorHandler handler);
}
