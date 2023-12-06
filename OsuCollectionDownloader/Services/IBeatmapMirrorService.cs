﻿using OsuCollectionDownloader.Objects;
using System.Text.Json;

namespace OsuCollectionDownloader.Services;

internal interface IBeatmapMirrorService
{
    Task<Result<object?>> SearchAsync(string query, CancellationToken token);

    Task<Result<bool>> DownloadAsync(string filePath, int beatmapSetId, CancellationToken token);
}