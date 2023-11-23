using OsuCollectionDownloader.Json.Models;
using OsuCollectionDownloader.Objects;
using OsuCollectionDownloader.Services;
using System.Collections.Immutable;

namespace OsuCollectionDownloader.Handlers;

internal sealed class OsuDirectMirrorHandler(IMirrorService service) : IMirrorHandler
{
    public async Task<Result<bool>> HandleAsync(string title, string difficultyName, string filePath, CancellationToken token)
    {
        Result<object?> searchResult = await service.SearchAsync(title, token);

        if (!searchResult.IsSuccessful ||
            !searchResult.HasValue ||
            searchResult.Value is not ImmutableArray<OsuDirectSearchResult?> value ||
            value.IsDefaultOrEmpty)
        {
            return false;
        }

        // Get the correct beatmapset id by checking the source and result difficulty names. If they're the same, they're referring to the same beatmap.
        int beatmapSetId = value
            .SelectMany(results => results!.Beatmaps)
            .Where(beatmap => beatmap.Version == difficultyName)
            .Select(correspondingBeatmap => correspondingBeatmap.BeatmapsetId)
            .FirstOrDefault();

        if (beatmapSetId == default)
        {
            return false;
        }

        return await service.DownloadAsync(filePath, beatmapSetId, token);
    }
}
