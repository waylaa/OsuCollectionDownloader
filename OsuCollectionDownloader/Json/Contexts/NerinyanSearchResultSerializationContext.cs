using OsuCollectionDownloader.Json.Models;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace OsuCollectionDownloader.Json.Contexts;

[JsonSerializable(typeof(ImmutableArray<NerinyanSearchResult?>))]
[JsonSerializable(typeof(NerinyanSearchResult))]
[JsonSerializable(typeof(NerinyanNominationsSummary))]
[JsonSerializable(typeof(NerinyanLanguage))]
[JsonSerializable(typeof(NerinyanHype))]
[JsonSerializable(typeof(NerinyanGenre))]
[JsonSerializable(typeof(NerinyanDiscussion))]
[JsonSerializable(typeof(NerinyanCache))]
[JsonSerializable(typeof(NerinyanBeatmap))]
[JsonSerializable(typeof(NerinyanAvailability))]
[JsonSerializable(typeof(ImmutableArray<NerinyanBeatmap>))]
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(DateTime?))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(int?))]
[JsonSerializable(typeof(double))]
internal sealed partial class NerinyanSearchResultSerializationContext : JsonSerializerContext
{
}
