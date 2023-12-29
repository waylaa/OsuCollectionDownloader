using OsuCollectionDownloader.Objects;
using System.Collections;

namespace OsuCollectionDownloader.Handlers.Chains;

internal sealed class MirrorChain : IEnumerable<IMirrorHandler>
{
    private readonly HashSet<IMirrorHandler> _mirrors = [];

    public IEnumerator<IMirrorHandler> GetEnumerator()
        => _mirrors.GetEnumerator();

    internal async Task<Result<bool>> HandleAsync(string filePath, int beatmapSetId, CancellationToken token)
    {
        if (_mirrors.Count == 0)
        {
            return false;
        }

        foreach (IMirrorHandler handler in _mirrors)
        {
            Result<bool> result = await handler.DownloadAsync(filePath, beatmapSetId, token);

            if (!result.IsSucessfulWithValue || !result.Value)
            {
                continue;
            }

            return result;
        }

        return false;
    }

    internal MirrorChain Add(IMirrorHandler handler)
    {
        _mirrors.Add(handler);
        return this;
    }

    IEnumerator IEnumerable.GetEnumerator()
        => _mirrors.GetEnumerator();
}
