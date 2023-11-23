using OsuCollectionDownloader.Objects;
using System.Text.Json;

namespace OsuCollectionDownloader.Services;

internal interface IMirrorService
{
    string BaseApiUrl { get; }

    Task<Result<object?>> SearchAsync(string query, CancellationToken token);

    Task<Result<bool>> DownloadAsync(string filePath, int beatmapSetId, CancellationToken token);
}
