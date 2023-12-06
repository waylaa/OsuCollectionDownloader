using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OsuCollectionDownloader.Json.Models;

internal sealed record ChildrenBeatmap
(
    [property: JsonPropertyName("BeatmapId")] int BeatmapId,
    [property: JsonPropertyName("ParentSetId")] int ParentSetId,
    [property: JsonPropertyName("DiffName")] string DiffName,
    [property: JsonPropertyName("FileMD5")] string FileMD5,
    [property: JsonPropertyName("Mode")] int Mode,
    [property: JsonPropertyName("BPM")] double BPM,
    [property: JsonPropertyName("AR")] double AR,
    [property: JsonPropertyName("OD")] double OD,
    [property: JsonPropertyName("CS")] double CS,
    [property: JsonPropertyName("HP")] double HP,
    [property: JsonPropertyName("TotalLength")] int TotalLength,
    [property: JsonPropertyName("HitLength")] int HitLength,
    [property: JsonPropertyName("Playcount")] int Playcount,
    [property: JsonPropertyName("Passcount")] int Passcount,
    [property: JsonPropertyName("MaxCombo")] int MaxCombo,
    [property: JsonPropertyName("DifficultyRating")] double DifficultyRating,
    [property: JsonPropertyName("OsuFile")] string OsuFile,
    [property: JsonPropertyName("DownloadPath")] string DownloadPath
);

internal sealed record Datum
(
    [property: JsonPropertyName("SetId")] int SetId,
    [property: JsonPropertyName("ChildrenBeatmaps")] ImmutableArray<ChildrenBeatmap> ChildrenBeatmaps,
    [property: JsonPropertyName("RankedStatus")] int RankedStatus,
    [property: JsonPropertyName("ApprovedDate")] string ApprovedDate,
    [property: JsonPropertyName("LastUpdate")] DateTime LastUpdate,
    [property: JsonPropertyName("LastChecked")] DateTime LastChecked,
    [property: JsonPropertyName("Artist")] string Artist,
    [property: JsonPropertyName("Title")] string Title,
    [property: JsonPropertyName("Creator")] string Creator,
    [property: JsonPropertyName("Source")] string Source,
    [property: JsonPropertyName("Tags")] string Tags,
    [property: JsonPropertyName("HasVideo")] bool HasVideo,
    [property: JsonPropertyName("Genre")] int Genre,
    [property: JsonPropertyName("Language")] int Language,
    [property: JsonPropertyName("Favourites")] int Favourites,
    [property: JsonPropertyName("Disabled")] bool Disabled
);

internal sealed record ChimuSearchResult([property: JsonPropertyName("data")] ImmutableArray<Datum> Data);
