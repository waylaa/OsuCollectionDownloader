using OsuCollectionDownloader.Json.Contexts;
using OsuCollectionDownloader.Json.Models;
using OsuCollectionDownloader.Objects;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace OsuCollectionDownloader.Services;

internal sealed class OsuCollectorService(HttpClient client)
{
    private const string BaseApiUrl = "https://osucollector.com/api";

    internal async Task<Result<FetchedCollectionMetadata?>> GetMetadataAsync(int collectionId, CancellationToken token)
    {
        try
        {
            return await client.GetFromJsonAsync
            (
                $"{BaseApiUrl}/collections/{collectionId}",
                FetchedCollectionMetadataSerializationContext.Default.FetchedCollectionMetadata,
                token
            );
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    internal async Task<Result<ImmutableList<Beatmap>>> GetBeatmapsAsync(int collectionId, CancellationToken ct)
    {
        try
        {
            List<Beatmap> beatmaps = [];

            FetchedCollection? collection = await client.GetFromJsonAsync
            (
                $"{BaseApiUrl}/collections/{collectionId}/beatmapsv2?cursor=0&perPage=100",
                FetchedCollectionSerializationContext.Default.FetchedCollection,
                ct
            );

            if (collection is null)
            {
                return ImmutableList<Beatmap>.Empty;
            }

            do
            {
                beatmaps.AddRange(collection.Beatmaps);

                collection = await client.GetFromJsonAsync
                (
                    $"{BaseApiUrl}/collections/{collectionId}/beatmapsv2?cursor={collection.NextPageCursor}&perPage=100",
                    FetchedCollectionSerializationContext.Default.FetchedCollection,
                    ct
                );

                if (collection is null)
                {
                    return ImmutableList<Beatmap>.Empty;
                }

            } while (collection.HasMore || collection.NextPageCursor is not null);

            return beatmaps.ToImmutableList();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}
