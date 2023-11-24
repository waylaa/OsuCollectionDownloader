using OsuCollectionDownloader.Json.Models;
using OsuCollectionDownloader.Objects;
using OsuCollectionDownloader.Services;

namespace OsuCollectionDownloader.Handlers;

internal sealed class ChimuHandler(IBeatmapMirrorService service) : IBeatmapMirrorHandler
{
    public async Task<Result<bool>> HandleAsync(string title, string difficultyName, string filePath, CancellationToken token)
    {
        Result<object?> searchResult = await service.SearchAsync(title, token);

        if (!searchResult.IsSuccessful ||
            !searchResult.HasValue ||
            searchResult.Value is not ChimuSearchResult value)
        {
            return false;
        }

        // Get the correct beatmapset id by checking the source and result difficulty names. If they're the same, they're referring to the same beatmap.
        int beatmapSetId = value.Data
            .SelectMany(results => results.ChildrenBeatmaps)
            .Where(beatmap => beatmap.DiffName == difficultyName)
            .Select(correspondingBeatmap => correspondingBeatmap.ParentSetId)
            .FirstOrDefault();

        if (beatmapSetId == default)
        {
            return false;
        }

        return await service.DownloadAsync(filePath, beatmapSetId, token);
    }
}
