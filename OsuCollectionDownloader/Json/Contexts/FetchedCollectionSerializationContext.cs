using OsuCollectionDownloader.Json.Models;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace OsuCollectionDownloader.Json.Contexts;

[JsonSerializable(typeof(FetchedCollection))]
[JsonSerializable(typeof(NominationsSummary))]
[JsonSerializable(typeof(Failtimes))]
[JsonSerializable(typeof(Covers))]
[JsonSerializable(typeof(Beatmapset))]
[JsonSerializable(typeof(Beatmap))]
[JsonSerializable(typeof(Availability))]
[JsonSerializable(typeof(IReadOnlyList<Beatmap>))]
[JsonSerializable(typeof(IReadOnlyList<int>))]
[JsonSerializable(typeof(IReadOnlyList<int>))]
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(int?))]
[JsonSerializable(typeof(double))]
internal sealed partial class FetchedCollectionSerializationContext : JsonSerializerContext
{
}
