using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace OsuCollectionDownloader.Json.Models;

internal sealed record PartialBeatmap
(
    [property: JsonPropertyName("checksum")] string Checksum,
    [property: JsonPropertyName("id")] int Id
);

internal sealed record PartialBeatmapset
(
    [property: JsonPropertyName("beatmaps")] IReadOnlyList<PartialBeatmap> Beatmaps,
    [property: JsonPropertyName("id")] int Id
);

internal sealed record BpmSpread
(
    [property: JsonPropertyName("150")] int _150,
    [property: JsonPropertyName("160")] int _160,
    [property: JsonPropertyName("170")] int _170,
    [property: JsonPropertyName("180")] int _180,
    [property: JsonPropertyName("190")] int _190,
    [property: JsonPropertyName("200")] int _200,
    [property: JsonPropertyName("210")] int _210,
    [property: JsonPropertyName("220")] int _220,
    [property: JsonPropertyName("230")] int _230,
    [property: JsonPropertyName("240")] int _240,
    [property: JsonPropertyName("250")] int _250,
    [property: JsonPropertyName("260")] int _260,
    [property: JsonPropertyName("270")] int _270,
    [property: JsonPropertyName("280")] int _280,
    [property: JsonPropertyName("290")] int _290,
    [property: JsonPropertyName("300")] int _300
);

internal sealed record Comment
(
    [property: JsonPropertyName("date")] Date Date,
    [property: JsonPropertyName("upvotes")] IReadOnlyList<int> Upvotes,
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("userId")] int UserId,
    [property: JsonPropertyName("username")] string Username
);

internal sealed record Date
(
    [property: JsonPropertyName("_seconds")] int Seconds,
    [property: JsonPropertyName("_nanoseconds")] int Nanoseconds
);

internal sealed record DateLastModified
(
    [property: JsonPropertyName("_seconds")] int Seconds,
    [property: JsonPropertyName("_nanoseconds")] int Nanoseconds
);

internal sealed record DateUploaded
(
    [property: JsonPropertyName("_seconds")] int Seconds,
    [property: JsonPropertyName("_nanoseconds")] int Nanoseconds
);

internal sealed record DifficultySpread
(
    [property: JsonPropertyName("1")] int _1,
    [property: JsonPropertyName("2")] int _2,
    [property: JsonPropertyName("3")] int _3,
    [property: JsonPropertyName("4")] int _4,
    [property: JsonPropertyName("5")] int _5,
    [property: JsonPropertyName("6")] int _6,
    [property: JsonPropertyName("7")] int _7,
    [property: JsonPropertyName("8")] int _8,
    [property: JsonPropertyName("9")] int _9,
    [property: JsonPropertyName("10")] int _10
);

internal sealed record Modes
(
    [property: JsonPropertyName("osu")] int Osu,
    [property: JsonPropertyName("taiko")] int Taiko,
    [property: JsonPropertyName("fruits")] int Fruits,
    [property: JsonPropertyName("mania")] int Mania
);

internal sealed record FetchedCollectionMetadata
(
    [property: JsonPropertyName("dateUploaded")] DateUploaded DateUploaded,
    [property: JsonPropertyName("uploader")] Uploader Uploader,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("beatmapCount")] int BeatmapCount,
    [property: JsonPropertyName("dateLastModified")] DateLastModified DateLastModified,
    [property: JsonPropertyName("unsubmittedBeatmapCount")] int UnsubmittedBeatmapCount,
    [property: JsonPropertyName("unknownChecksums")] IReadOnlyList<object> UnknownChecksums,
    [property: JsonPropertyName("beatmapsets")] IReadOnlyList<Beatmapset> Beatmapsets,
    [property: JsonPropertyName("modes")] Modes Modes,
    [property: JsonPropertyName("difficultySpread")] DifficultySpread DifficultySpread,
    [property: JsonPropertyName("bpmSpread")] BpmSpread BpmSpread,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("comments")] IReadOnlyList<Comment> Comments,
    [property: JsonPropertyName("favouritedBy")] IReadOnlyList<int> FavouritedBy,
    [property: JsonPropertyName("favourites")] int Favourites
);

internal sealed record Uploader
(
    [property: JsonPropertyName("avatarURL")] string AvatarURL,
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("username")] string Username
);
