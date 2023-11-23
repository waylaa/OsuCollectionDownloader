using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace OsuCollectionDownloader.Json.Models;

internal sealed record NerinyanAvailability(
    [property: JsonPropertyName("download_disabled")] bool DownloadDisabled,
    [property: JsonPropertyName("more_information")] object MoreInformation
);

internal sealed record NerinyanBeatmap(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("beatmapset_id")] int BeatmapsetId,
    [property: JsonPropertyName("mode")] string Mode,
    [property: JsonPropertyName("mode_int")] int ModeInt,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("ranked")] int Ranked,
    [property: JsonPropertyName("total_length")] int TotalLength,
    [property: JsonPropertyName("max_combo")] int MaxCombo,
    [property: JsonPropertyName("difficulty_rating")] double DifficultyRating,
    [property: JsonPropertyName("version")] string Version,
    [property: JsonPropertyName("accuracy")] double Accuracy,
    [property: JsonPropertyName("ar")] double Ar,
    [property: JsonPropertyName("cs")] double Cs,
    [property: JsonPropertyName("drain")] double Drain,
    [property: JsonPropertyName("bpm")] double Bpm,
    [property: JsonPropertyName("convert")] bool Convert,
    [property: JsonPropertyName("count_circles")] int CountCircles,
    [property: JsonPropertyName("count_sliders")] int CountSliders,
    [property: JsonPropertyName("count_spinners")] int CountSpinners,
    [property: JsonPropertyName("hit_length")] int HitLength,
    [property: JsonPropertyName("is_scoreable")] bool IsScoreable,
    [property: JsonPropertyName("last_updated")] DateTime LastUpdated,
    [property: JsonPropertyName("deleted_at")] object DeletedAt,
    [property: JsonPropertyName("passcount")] int Passcount,
    [property: JsonPropertyName("playcount")] int Playcount,
    [property: JsonPropertyName("checksum")] string Checksum,
    [property: JsonPropertyName("user_id")] int UserId,
    [property: JsonPropertyName("osu_file")] string OsuFile
);

internal sealed record NerinyanCache(
    [property: JsonPropertyName("video")] bool Video,
    [property: JsonPropertyName("noVideo")] bool NoVideo
);

internal sealed  record NerinyanDiscussion(
    [property: JsonPropertyName("enabled")] bool Enabled,
    [property: JsonPropertyName("locked")] bool Locked
);

internal sealed record NerinyanGenre(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name
);

internal sealed record NerinyanHype(
    [property: JsonPropertyName("current")] int? Current,
    [property: JsonPropertyName("required")] int? Required
);

internal sealed record NerinyanLanguage(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name
);

internal sealed record NerinyanNominationsSummary(
    [property: JsonPropertyName("current")] int Current,
    [property: JsonPropertyName("required")] int Required
);

internal sealed record NerinyanSearchResult(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("artist")] string Artist,
    [property: JsonPropertyName("artist_unicode")] string ArtistUnicode,
    [property: JsonPropertyName("creator")] string Creator,
    [property: JsonPropertyName("favourite_count")] int FavouriteCount,
    [property: JsonPropertyName("hype")] NerinyanHype Hype,
    [property: JsonPropertyName("nsfw")] bool Nsfw,
    [property: JsonPropertyName("play_count")] int PlayCount,
    [property: JsonPropertyName("source")] string Source,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("title_unicode")] string TitleUnicode,
    [property: JsonPropertyName("user_id")] int UserId,
    [property: JsonPropertyName("video")] bool Video,
    [property: JsonPropertyName("availability")] NerinyanAvailability Availability,
    [property: JsonPropertyName("bpm")] double Bpm,
    [property: JsonPropertyName("can_be_hyped")] bool CanBeHyped,
    [property: JsonPropertyName("discussion")] NerinyanDiscussion Discussion,
    [property: JsonPropertyName("is_scoreable")] bool IsScoreable,
    [property: JsonPropertyName("last_updated")] DateTime LastUpdated,
    [property: JsonPropertyName("deleted_at")] object DeletedAt,
    [property: JsonPropertyName("legacy_thread_url")] string LegacyThreadUrl,
    [property: JsonPropertyName("nominations_summary")] NerinyanNominationsSummary NominationsSummary,
    [property: JsonPropertyName("ranked")] int Ranked,
    [property: JsonPropertyName("ranked_date")] DateTime? RankedDate,
    [property: JsonPropertyName("storyboard")] bool Storyboard,
    [property: JsonPropertyName("submitted_date")] DateTime SubmittedDate,
    [property: JsonPropertyName("tags")] string Tags,
    [property: JsonPropertyName("has_favourited")] bool HasFavourited,
    [property: JsonPropertyName("beatmaps")] ImmutableArray<NerinyanBeatmap> Beatmaps,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("genre")] NerinyanGenre Genre,
    [property: JsonPropertyName("language")] NerinyanLanguage Language,
    [property: JsonPropertyName("ratings_string")] string RatingsString,
    [property: JsonPropertyName("cache")] NerinyanCache Cache
);
