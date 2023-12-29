using OsuCollectionDownloader.Handlers.Abstractions;
using OsuCollectionDownloader.Objects;

namespace OsuCollectionDownloader.Handlers;

internal sealed class ChimuHandler(HttpClient client) : IMirrorHandler
{
    public async Task<Result<bool>> DownloadAsync(string filePath, int beatmapSetId, CancellationToken token)
    {
        try
        {
            using HttpResponseMessage response = await client.GetAsync($"https://api.chimu.moe/v1/download/{beatmapSetId}", HttpCompletionOption.ResponseHeadersRead, token);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            await using FileStream beatmapStream = File.OpenWrite(filePath);
            await response.Content.CopyToAsync(beatmapStream, token);

            return true;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or IOException)
        {
            return ex;
        }
    }
}
