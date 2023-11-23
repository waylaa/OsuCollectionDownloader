using OsuCollectionDownloader.Json.Models;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace OsuCollectionDownloader.Json.Contexts;

[JsonSerializable(typeof(ImmutableArray<OsuDirectSearchResult?>))]
[JsonSerializable(typeof(OsuDirectSearchResult))]
[JsonSerializable(typeof(OsuDirectNominationsSummary))]
[JsonSerializable(typeof(OsuDirectHype))]
[JsonSerializable(typeof(OsuDirectCovers))]
[JsonSerializable(typeof(OsuDirectBeatmap))]
[JsonSerializable(typeof(OsuDirectAvailability))]
[JsonSerializable(typeof(ImmutableArray<OsuDirectBeatmap>))]
[JsonSerializable(typeof(ImmutableArray<string>))]
[JsonSerializable(typeof(ImmutableArray<int>))]
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(DateTime?))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(int?))]
[JsonSerializable(typeof(double))]
internal partial class OsuDirectSearchResultSerializationContext : JsonSerializerContext
{
}
