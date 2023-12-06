using OsuCollectionDownloader.Objects;
using System.Collections;

namespace OsuCollectionDownloader.Handlers.Chains;

internal sealed class MirrorChain : IEnumerable<IBeatmapMirrorHandler>
{
    private readonly HashSet<IBeatmapMirrorHandler> _mirrors = [];

    public IEnumerator<IBeatmapMirrorHandler> GetEnumerator()
        => _mirrors.GetEnumerator();

    internal async Task<Result<bool>> HandleAsync(string title, string difficultyName, string filePath, CancellationToken token)
    {
        if (_mirrors.Count == 0)
        {
            return false;
        }

        foreach (IBeatmapMirrorHandler handler in _mirrors)
        {
            Result<bool> result = await handler.HandleAsync(title, difficultyName, filePath, token);

            if (!result.IsSucessfulWithValue || !result.Value)
            {
                continue;
            }

            return result;
        }

        return false;
    }

    internal MirrorChain Add(IBeatmapMirrorHandler handler)
    {
        _mirrors.Add(handler);
        return this;
    }

    IEnumerator IEnumerable.GetEnumerator()
        => _mirrors.GetEnumerator();
}
