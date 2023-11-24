using OsuCollectionDownloader.Json.Contexts;
using OsuCollectionDownloader.Objects;
using System.Net.Http.Json;

namespace OsuCollectionDownloader.Services;

internal sealed class ChimuService(HttpClient client) : IBeatmapMirrorService
{
    private const string BaseApiUrl = "https://api.chimu.moe/v1";

    public async Task<Result<bool>> DownloadAsync(string filePath, int beatmapSetId, CancellationToken token)
    {
        try
        {
            using HttpResponseMessage response = await client.GetAsync($"{BaseApiUrl}/download/{beatmapSetId}", HttpCompletionOption.ResponseHeadersRead, token);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            await using FileStream beatmapStream = File.OpenWrite(filePath);
            await response.Content.CopyToAsync(beatmapStream, token);

            return true;
        }
        catch (Exception ex) when (ex is IOException or HttpRequestException)
        {
            return ex;
        }
    }

    public async Task<Result<object?>> SearchAsync(string query, CancellationToken token)
    {
        try
        {
            return await client.GetFromJsonAsync
            (
                $"{BaseApiUrl}/search?size=40&offset=0&query={Uri.EscapeDataString(query)}",
                ChimuSearchResultSerializationContext.Default.ChimuSearchResult,
                token
            );
        }
        catch (HttpRequestException ex)
        {
            return ex;
        }
    }
}
