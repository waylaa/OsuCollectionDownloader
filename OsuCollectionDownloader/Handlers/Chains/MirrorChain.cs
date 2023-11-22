using OsuCollectionDownloader.Objects;
using System.Collections;

namespace OsuCollectionDownloader.Handlers.Chains;

internal sealed class MirrorChain : MirrorChainBase, IEnumerable<IMirrorHandler>
{
    protected override ISet<IMirrorHandler> Mirrors { get; } = new HashSet<IMirrorHandler>();

    public IEnumerator<IMirrorHandler> GetEnumerator()
        => Mirrors.GetEnumerator();

    public override async Task<Result<bool>> HandleAsync(string title, string difficultyName, string filePath, CancellationToken token)
    {
        if (Mirrors.Count == 0)
        {
            return false;
        }

        foreach (IMirrorHandler handler in Mirrors)
        {
            Result<bool> result = await handler.HandleAsync(title, difficultyName, filePath, token);

            if (!result.IsSuccessful || !result.Value)
            {
                continue;
            }

            return result;
        }

        return false;
    }

    protected internal override MirrorChainBase Add(IMirrorHandler handler)
    {
        Mirrors.Add(handler);
        return this;
    }

    IEnumerator IEnumerable.GetEnumerator()
        => ((IEnumerable)Mirrors).GetEnumerator();
}
