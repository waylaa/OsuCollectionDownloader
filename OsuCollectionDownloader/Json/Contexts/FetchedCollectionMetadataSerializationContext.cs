using OsuCollectionDownloader.Json.Models;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace OsuCollectionDownloader.Json.Contexts;

[JsonSerializable(typeof(FetchedCollectionMetadata))]
[JsonSerializable(typeof(Uploader))]
[JsonSerializable(typeof(Modes))]
[JsonSerializable(typeof(DifficultySpread))]
[JsonSerializable(typeof(DateUploaded))]
[JsonSerializable(typeof(DateLastModified))]
[JsonSerializable(typeof(Date))]
[JsonSerializable(typeof(Comment))]
[JsonSerializable(typeof(BpmSpread))]
[JsonSerializable(typeof(PartialBeatmapset))]
[JsonSerializable(typeof(PartialBeatmap))]
[JsonSerializable(typeof(IReadOnlyList<PartialBeatmap>))]
[JsonSerializable(typeof(IReadOnlyList<Beatmapset>))]
[JsonSerializable(typeof(IReadOnlyList<Comment>))]
[JsonSerializable(typeof(IReadOnlyList<int>))]
[JsonSerializable(typeof(IReadOnlyList<object>))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(int))]
internal sealed partial class FetchedCollectionMetadataSerializationContext : JsonSerializerContext
{
}
