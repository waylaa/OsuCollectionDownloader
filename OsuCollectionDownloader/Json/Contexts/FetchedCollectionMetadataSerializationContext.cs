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
[JsonSerializable(typeof(IImmutableList<PartialBeatmap>))]
[JsonSerializable(typeof(IImmutableList<Beatmapset>))]
[JsonSerializable(typeof(IImmutableList<Comment>))]
[JsonSerializable(typeof(IImmutableList<int>))]
[JsonSerializable(typeof(IImmutableList<object>))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(int))]
internal sealed partial class FetchedCollectionMetadataSerializationContext : JsonSerializerContext
{
}
