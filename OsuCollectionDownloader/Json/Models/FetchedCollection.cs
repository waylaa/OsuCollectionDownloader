using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace OsuCollectionDownloader.Json.Models;

internal sealed record Availability
(
    [property: JsonPropertyName("download_disabled")] bool DownloadDisabled,
    [property: JsonPropertyName("more_information")] object MoreInformation
);

internal sealed record Beatmap
(
    [property: JsonPropertyName("difficulty_rating")] double DifficultyRating,
    [property: JsonPropertyName("count_sliders")] int CountSliders,
    [property: JsonPropertyName("mode_int")] int ModeInt,
    [property: JsonPropertyName("accuracy")] double Accuracy,
    [property: JsonPropertyName("convert")] bool Convert,
    [property: JsonPropertyName("failtimes")] Failtimes Failtimes,
    [property: JsonPropertyName("passcount")] int Passcount,
    [property: JsonPropertyName("drain")] double Drain,
    [property: JsonPropertyName("mode")] string Mode,
    [property: JsonPropertyName("is_scoreable")] bool IsScoreable,
    [property: JsonPropertyName("playcount")] int Playcount,
    [property: JsonPropertyName("max_combo")] int? MaxCombo,
    [property: JsonPropertyName("checksum")] string Checksum,
    [property: JsonPropertyName("ranked")] int Ranked,
    [property: JsonPropertyName("total_length")] int TotalLength,
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("bpm")] double Bpm,
    [property: JsonPropertyName("beatmapset_id")] int BeatmapsetId,
    [property: JsonPropertyName("last_updated")] DateTime LastUpdated,
    [property: JsonPropertyName("count_spinners")] int CountSpinners,
    [property: JsonPropertyName("version")] string Version,
    [property: JsonPropertyName("deleted_at")] object DeletedAt,
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("cs")] double Cs,
    [property: JsonPropertyName("count_circles")] int CountCircles,
    [property: JsonPropertyName("ar")] double Ar,
    [property: JsonPropertyName("beatmapset")] Beatmapset Beatmapset,
    [property: JsonPropertyName("user_id")] int UserId,
    [property: JsonPropertyName("hit_length")] int HitLength,
    [property: JsonPropertyName("status")] string Status
);

internal sealed record Beatmapset
(
    [property: JsonPropertyName("submitted_date")] DateTime SubmittedDate,
    [property: JsonPropertyName("nominations_summary")] NominationsSummary NominationsSummary,
    [property: JsonPropertyName("discussion_locked")] bool DiscussionLocked,
    [property: JsonPropertyName("artist")] string Artist,
    [property: JsonPropertyName("artist_unicode")] string ArtistUnicode,
    [property: JsonPropertyName("video")] bool Video,
    [property: JsonPropertyName("source")] string Source,
    [property: JsonPropertyName("availability")] Availability Availability,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("discussion_enabled")] bool DiscussionEnabled,
    [property: JsonPropertyName("is_scoreable")] bool IsScoreable,
    [property: JsonPropertyName("can_be_hyped")] bool CanBeHyped,
    [property: JsonPropertyName("ratings")] IReadOnlyList<int> Ratings,
    [property: JsonPropertyName("ranked")] int Ranked,
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("legacy_thread_url")] string LegacyThreadUrl,
    [property: JsonPropertyName("bpm")] double Bpm,
    [property: JsonPropertyName("covers")] Covers Covers,
    [property: JsonPropertyName("creator")] string Creator,
    [property: JsonPropertyName("last_updated")] DateTime LastUpdated,
    [property: JsonPropertyName("nsfw")] bool Nsfw,
    [property: JsonPropertyName("play_count")] int PlayCount,
    [property: JsonPropertyName("storyboard")] bool Storyboard,
    [property: JsonPropertyName("tags")] string Tags,
    [property: JsonPropertyName("user_id")] int UserId,
    [property: JsonPropertyName("ranked_date")] DateTime? RankedDate,
    [property: JsonPropertyName("preview_url")] string PreviewUrl,
    [property: JsonPropertyName("favourite_count")] int FavouriteCount,
    [property: JsonPropertyName("hype")] object Hype,
    [property: JsonPropertyName("title_unicode")] string TitleUnicode,
    [property: JsonPropertyName("status")] string Status
);

internal sealed record Covers
(
    [property: JsonPropertyName("slimcover")] string Slimcover,
    [property: JsonPropertyName("cover@2x")] string Cover2x,
    [property: JsonPropertyName("list")] string List,
    [property: JsonPropertyName("list@2x")] string List2x,
    [property: JsonPropertyName("cover")] string Cover,
    [property: JsonPropertyName("card@2x")] string Card2x,
    [property: JsonPropertyName("card")] string Card,
    [property: JsonPropertyName("slimcover@2x")] string Slimcover2x
);

internal sealed record Failtimes
(
    [property: JsonPropertyName("exit")] IReadOnlyList<int> Exit,
    [property: JsonPropertyName("fail")] IReadOnlyList<int> Fail
);

internal sealed record NominationsSummary
(
    [property: JsonPropertyName("required")] int Required,
    [property: JsonPropertyName("current")] int Current
);

internal sealed record FetchedCollection
(
    [property: JsonPropertyName("nextPageCursor")] int? NextPageCursor,
    [property: JsonPropertyName("hasMore")] bool HasMore,
    [property: JsonPropertyName("beatmaps")] IReadOnlyList<Beatmap> Beatmaps
);
