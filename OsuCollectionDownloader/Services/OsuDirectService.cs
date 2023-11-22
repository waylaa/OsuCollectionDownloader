using OsuCollectionDownloader.Objects;
using System.Net.Http.Json;
using System.Text.Json;

namespace OsuCollectionDownloader.Services;

internal sealed class OsuDirectService(HttpClient client) : IMirrorService
{
    public string BaseApiUrl => "https://api.osu.direct";

    public async Task<Result<bool>> DownloadAsync(string filePath, int beatmapSetId, CancellationToken token)
    {
        try
        {
            using HttpResponseMessage response = await client.GetAsync($"{BaseApiUrl}/d/{beatmapSetId}", HttpCompletionOption.ResponseHeadersRead, token);

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

    public async Task<Result<JsonDocument?>> SearchAsync(string query, CancellationToken token)
    {
        try
        {
            return await client.GetFromJsonAsync<JsonDocument>($"{BaseApiUrl}/v2/search?amount=100&offset=0&q={Uri.EscapeDataString(query)}&mode=&status=", token);
        }
        catch (HttpRequestException ex)
        {
            return ex;
        }
    }
}
