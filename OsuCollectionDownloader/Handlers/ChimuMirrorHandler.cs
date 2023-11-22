using OsuCollectionDownloader.Objects;
using OsuCollectionDownloader.Services;
using System.Text.Json;

namespace OsuCollectionDownloader.Handlers;

internal sealed class ChimuMirrorHandler(IMirrorService service) : IMirrorHandler
{
    public async Task<Result<bool>> HandleAsync(string title, string difficultyName, string filePath, CancellationToken token)
    {
        Result<JsonDocument?> searchResult = await service.SearchAsync(title, token);

        if (!searchResult.IsSuccessful || !searchResult.HasValue)
        {
            return false;
        }

        // Get the correct beatmapset id by checking the source and result difficulty names. If they're the same, they're referring to the same beatmap.
        int beatmapSetId = searchResult.Value!.RootElement
            .GetProperty("data")
            .EnumerateArray()
            .SelectMany(results => results.GetProperty("ChildrenBeatmaps").EnumerateArray())
            .Where(beatmap => beatmap.GetProperty("DiffName").GetString() == difficultyName)
            .Select(correspondingBeatmap => correspondingBeatmap.GetProperty("ParentSetId").GetInt32())
            .FirstOrDefault();

        if (beatmapSetId == default)
        {
            return false;
        }

        return await service.DownloadAsync(filePath, beatmapSetId, token);
    }
}
