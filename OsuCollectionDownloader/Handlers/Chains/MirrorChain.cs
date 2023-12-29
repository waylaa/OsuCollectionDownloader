using OsuCollectionDownloader.Handlers.Abstractions;
using OsuCollectionDownloader.Objects;
using System.Collections;
using System.Collections.ObjectModel;

namespace OsuCollectionDownloader.Handlers.Chains;

internal sealed class MirrorChain : IReadOnlyCollection<IMirrorHandler>
{
    private readonly List<IMirrorHandler> _mirrors = [];

    public int Count => _mirrors.Count;

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
