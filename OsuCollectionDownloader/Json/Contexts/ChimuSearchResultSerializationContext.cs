using OsuCollectionDownloader.Json.Models;
using System.Text.Json.Serialization;

namespace OsuCollectionDownloader.Json.Contexts;

[JsonSerializable(typeof(ChimuSearchResult))]
[JsonSerializable(typeof(Datum))]
[JsonSerializable(typeof(ChildrenBeatmap))]
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(double))]
internal partial class ChimuSearchResultSerializationContext : JsonSerializerContext
{
}
