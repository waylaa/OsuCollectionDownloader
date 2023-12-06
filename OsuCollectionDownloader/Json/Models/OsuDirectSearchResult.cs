using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace OsuCollectionDownloader.Json.Models;

internal sealed record OsuDirectAvailability
(
    [property: JsonPropertyName("download_disabled")] bool DownloadDisabled,
    [property: JsonPropertyName("more_information")] object MoreInformation
);

internal sealed record OsuDirectBeatmap
(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("beatmapset_id")] int BeatmapsetId,
    [property: JsonPropertyName("difficulty_rating")] double DifficultyRating,
    [property: JsonPropertyName("mode")] string Mode,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("user_id")] int UserId,
    [property: JsonPropertyName("version")] string Version,
    [property: JsonPropertyName("accuracy")] double Accuracy,
    [property: JsonPropertyName("ar")] double Ar,
    [property: JsonPropertyName("bpm")] double Bpm,
    [property: JsonPropertyName("convert")] bool Convert,
    [property: JsonPropertyName("count_circles")] int CountCircles,
    [property: JsonPropertyName("count_sliders")] int CountSliders,
    [property: JsonPropertyName("count_spinners")] int CountSpinners,
    [property: JsonPropertyName("cs")] double Cs,
    [property: JsonPropertyName("deleted_at")] object DeletedAt,
    [property: JsonPropertyName("drain")] double Drain,
    [property: JsonPropertyName("hit_length")] int HitLength,
    [property: JsonPropertyName("is_scoreable")] bool IsScoreable,
    [property: JsonPropertyName("last_updated")] DateTime LastUpdated,
    [property: JsonPropertyName("mode_int")] int ModeInt,
    [property: JsonPropertyName("passcount")] int Passcount,
    [property: JsonPropertyName("playcount")] int Playcount,
    [property: JsonPropertyName("ranked")] int Ranked,
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("checksum")] string Checksum,
    [property: JsonPropertyName("max_combo")] int? MaxCombo
);

internal sealed record OsuDirectCovers
(
    [property: JsonPropertyName("cover")] string Cover,
    [property: JsonPropertyName("cover@2x")] string Cover2x,
    [property: JsonPropertyName("card")] string Card,
    [property: JsonPropertyName("card@2x")] string Card2x,
    [property: JsonPropertyName("list")] string List,
    [property: JsonPropertyName("list@2x")] string List2x,
    [property: JsonPropertyName("slimcover")] string Slimcover,
    [property: JsonPropertyName("slimcover@2x")] string Slimcover2x
);

internal sealed record OsuDirectHype
(
    [property: JsonPropertyName("current")] int Current,
    [property: JsonPropertyName("required")] int Required
);

internal sealed record OsuDirectNominationsSummary
(
    [property: JsonPropertyName("current")] int Current,
    [property: JsonPropertyName("required")] int Required
);

internal sealed record OsuDirectSearchResult
(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("title_unicode")] string TitleUnicode,
    [property: JsonPropertyName("artist")] string Artist,
    [property: JsonPropertyName("artist_unicode")] string ArtistUnicode,
    [property: JsonPropertyName("creator")] string Creator,
    [property: JsonPropertyName("source")] string Source,
    [property: JsonPropertyName("tags")] string Tags,
    [property: JsonPropertyName("covers")] OsuDirectCovers Covers,
    [property: JsonPropertyName("favourite_count")] int FavouriteCount,
    [property: JsonPropertyName("hype")] OsuDirectHype Hype,
    [property: JsonPropertyName("nsfw")] bool Nsfw,
    [property: JsonPropertyName("offset")] int Offset,
    [property: JsonPropertyName("play_count")] int PlayCount,
    [property: JsonPropertyName("spotlight")] bool Spotlight,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("track_id")] object TrackId,
    [property: JsonPropertyName("user_id")] int UserId,
    [property: JsonPropertyName("video")] bool Video,
    [property: JsonPropertyName("bpm")] double Bpm,
    [property: JsonPropertyName("can_be_hyped")] bool CanBeHyped,
    [property: JsonPropertyName("deleted_at")] object DeletedAt,
    [property: JsonPropertyName("discussion_enabled")] bool DiscussionEnabled,
    [property: JsonPropertyName("discussion_locked")] bool DiscussionLocked,
    [property: JsonPropertyName("is_scoreable")] bool IsScoreable,
    [property: JsonPropertyName("last_updated")] DateTime LastUpdated,
    [property: JsonPropertyName("legacy_thread_url")] string LegacyThreadUrl,
    [property: JsonPropertyName("nominations_summary")] OsuDirectNominationsSummary NominationsSummary,
    [property: JsonPropertyName("ranked")] int Ranked,
    [property: JsonPropertyName("ranked_date")] DateTime? RankedDate,
    [property: JsonPropertyName("availability")] OsuDirectAvailability Availability,
    [property: JsonPropertyName("has_favourited")] bool HasFavourited,
    [property: JsonPropertyName("beatmaps")] ImmutableArray<OsuDirectBeatmap> Beatmaps,
    [property: JsonPropertyName("pack_tags")] ImmutableArray<string> PackTags,
    [property: JsonPropertyName("modes")] ImmutableArray<int> Modes
);
