using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace OsuCollectionDownloader.Legacy.Objects;

internal sealed record Availability(
    [property: JsonPropertyName("more_information")] object MoreInformation,
    [property: JsonPropertyName("download_disabled")] bool DownloadDisabled
);

internal sealed record Beatmap(
    [property: JsonPropertyName("accuracy")] double Accuracy,
    [property: JsonPropertyName("ar")] double Ar,
    [property: JsonPropertyName("beatmapset")] Beatmapset Beatmapset,
    [property: JsonPropertyName("beatmapset_id")] int BeatmapsetId,
    [property: JsonPropertyName("bpm")] double Bpm,
    [property: JsonPropertyName("checksum")] string Checksum,
    [property: JsonPropertyName("convert")] bool? Convert,
    [property: JsonPropertyName("count_circles")] int CountCircles,
    [property: JsonPropertyName("count_sliders")] int CountSliders,
    [property: JsonPropertyName("count_spinners")] int CountSpinners,
    [property: JsonPropertyName("cs")] double Cs,
    [property: JsonPropertyName("deleted_at")] object DeletedAt,
    [property: JsonPropertyName("difficulty_rating")] double DifficultyRating,
    [property: JsonPropertyName("drain")] double Drain,
    [property: JsonPropertyName("failtimes")] Failtimes Failtimes,
    [property: JsonPropertyName("hit_length")] int HitLength,
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("is_scoreable")] bool? IsScoreable,
    [property: JsonPropertyName("last_updated")] DateTime LastUpdated,
    [property: JsonPropertyName("max_combo")] int? MaxCombo,
    [property: JsonPropertyName("mode")] string Mode,
    [property: JsonPropertyName("mode_int")] int ModeInt,
    [property: JsonPropertyName("passcount")] int? Passcount,
    [property: JsonPropertyName("playcount")] int? Playcount,
    [property: JsonPropertyName("ranked")] int Ranked,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("total_length")] int TotalLength,
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("user_id")] int UserId,
    [property: JsonPropertyName("version")] string Version
);

internal sealed record Beatmapset(
    [property: JsonPropertyName("submitted_date")] DateTime SubmittedDate,
    [property: JsonPropertyName("nominations_summary")] NominationsSummary NominationsSummary,
    [property: JsonPropertyName("creator")] string Creator,
    [property: JsonPropertyName("offset")] int Offset,
    [property: JsonPropertyName("artist")] string Artist,
    [property: JsonPropertyName("artist_unicode")] string ArtistUnicode,
    [property: JsonPropertyName("video")] bool Video,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("user_id")] int UserId,
    [property: JsonPropertyName("ratings")] IImmutableList<int> Ratings,
    [property: JsonPropertyName("preview_url")] string PreviewUrl,
    [property: JsonPropertyName("ranked_date")] DateTime? RankedDate,
    [property: JsonPropertyName("track_id")] int? TrackId,
    [property: JsonPropertyName("ranked")] int Ranked,
    [property: JsonPropertyName("title_unicode")] string TitleUnicode,
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("bpm")] double Bpm,
    [property: JsonPropertyName("spotlight")] bool Spotlight,
    [property: JsonPropertyName("covers")] Covers Covers,
    [property: JsonPropertyName("discussion_locked")] bool? DiscussionLocked,
    [property: JsonPropertyName("source")] string Source,
    [property: JsonPropertyName("availability")] Availability Availability,
    [property: JsonPropertyName("discussion_enabled")] bool? DiscussionEnabled,
    [property: JsonPropertyName("is_scoreable")] bool? IsScoreable,
    [property: JsonPropertyName("can_be_hyped")] bool? CanBeHyped,
    [property: JsonPropertyName("legacy_thread_url")] string LegacyThreadUrl,
    [property: JsonPropertyName("last_updated")] DateTime? LastUpdated,
    [property: JsonPropertyName("nsfw")] bool? Nsfw,
    [property: JsonPropertyName("play_count")] int? PlayCount,
    [property: JsonPropertyName("deleted_at")] object DeletedAt,
    [property: JsonPropertyName("storyboard")] bool? Storyboard,
    [property: JsonPropertyName("tags")] string Tags,
    [property: JsonPropertyName("favourite_count")] int? FavouriteCount,
    [property: JsonPropertyName("hype")] Hype Hype,
    [property: JsonPropertyName("status")] string Status
);

internal sealed record Covers(
    [property: JsonPropertyName("cover")] string Cover,
    [property: JsonPropertyName("list@2x")] string List2x,
    [property: JsonPropertyName("card@2x")] string Card2x,
    [property: JsonPropertyName("cover@2x")] string Cover2x,
    [property: JsonPropertyName("slimcover")] string Slimcover,
    [property: JsonPropertyName("list")] string List,
    [property: JsonPropertyName("card")] string Card,
    [property: JsonPropertyName("slimcover@2x")] string Slimcover2x
);

internal sealed record Failtimes(
    [property: JsonPropertyName("fail")] IImmutableList<int> Fail,
    [property: JsonPropertyName("exit")] IImmutableList<int> Exit
);

internal sealed record Hype(
    [property: JsonPropertyName("current")] int Current,
    [property: JsonPropertyName("required")] int Required
);

internal sealed record NominationsSummary(
    [property: JsonPropertyName("current")] int Current,
    [property: JsonPropertyName("required")] int Required
);

internal sealed record FetchedCollection(
    [property: JsonPropertyName("nextPageCursor")] int? NextPageCursor,
    [property: JsonPropertyName("hasMore")] bool HasMore,
    [property: JsonPropertyName("beatmaps")] IImmutableList<Beatmap> Beatmaps
);
