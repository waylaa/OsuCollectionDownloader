using OsuCollectionDownloader.Objects;
using System.Collections;

namespace OsuCollectionDownloader.Handlers.Chains;

internal sealed class MirrorChain : IEnumerable<IServiceHandler>
{
    private ISet<IServiceHandler> Mirrors { get; } = new HashSet<IServiceHandler>();

    public IEnumerator<IServiceHandler> GetEnumerator()
        => Mirrors.GetEnumerator();

    public async Task<Result<bool>> HandleAsync(string title, string difficultyName, string filePath, CancellationToken token)
    {
        if (Mirrors.Count == 0)
        {
            return false;
        }

        foreach (IServiceHandler handler in Mirrors)
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

    internal MirrorChain Add(IServiceHandler handler)
    {
        Mirrors.Add(handler);
        return this;
    }

    IEnumerator IEnumerable.GetEnumerator()
        => ((IEnumerable)Mirrors).GetEnumerator();
}
