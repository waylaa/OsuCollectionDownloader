using Microsoft.Extensions.Options;
using OsuCollectionDownloader.Cache;
using OsuCollectionDownloader.Extensions;
using OsuCollectionDownloader.Json.Contexts;
using OsuCollectionDownloader.Json.Models;
using OsuCollectionDownloader.Objects;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO.Compression;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OsuCollectionDownloader.Processors;

internal abstract class DownloadProcessorBase(DownloadProcessorBaseOptions options, IHttpClientFactory clientFactory)
{
    protected DownloadProcessorBaseOptions Options { get; } = options;

    protected HttpClient Client { get; } =  clientFactory.CreateClient();

    private const string OsuCollectorApiUrl = "https://osucollector.com/api";

    internal abstract Task DownloadAsync(CancellationToken token);

    private protected async Task<Result<FetchedCollectionMetadata?>> GetMetadataAsync(CancellationToken token)
    {
        try
        {
            return await Client.GetFromJsonAsync
            (
                $"{OsuCollectorApiUrl}/collections/{Options.Id}",
                FetchedCollectionMetadataSerializationContext.Default.FetchedCollectionMetadata,
                token
            );
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private protected Result<bool> Extract(string sourceBeatmapFilePath)
    {
        try
        {
            ZipFile.ExtractToDirectory
            (
                sourceBeatmapFilePath,
                Path.Combine(Options.ExtractionDirectory, Path.GetFileNameWithoutExtension(sourceBeatmapFilePath)),
                overwriteFiles: true
            );

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private protected async Task<Result<ImmutableList<Beatmap>>> GetBeatmapsAsync(CancellationToken token)
    {
        try
        {
            List<Beatmap> beatmaps = [];

            FetchedCollection? collection = await Client.GetFromJsonAsync
            (
                $"{OsuCollectorApiUrl}/collections/{Options.Id}/beatmapsv2?cursor=0&perPage=100",
                FetchedCollectionSerializationContext.Default.FetchedCollection,
                token
            );

            if (collection is null)
            {
                return ImmutableList<Beatmap>.Empty;
            }

            do
            {
                beatmaps.AddRange(collection.Beatmaps);

                collection = await Client.GetFromJsonAsync
                (
                    $"{OsuCollectorApiUrl}/collections/{Options.Id}/beatmapsv2?cursor={collection.NextPageCursor}&perPage=100",
                    FetchedCollectionSerializationContext.Default.FetchedCollection,
                    token
                );

                if (collection is null)
                {
                    return ImmutableList<Beatmap>.Empty;
                }

            } while (collection.HasMore || collection.NextPageCursor is not null || token.IsCancellationRequested);

            return beatmaps.ToImmutableList();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private protected async ValueTask GenerateOsdbCollectionAsync(FetchedCollectionMetadata metadata, ImmutableList<Beatmap> downloadedBeatmaps)
    {
        string osdbFilePath = Path.Combine(Options.OsdbGenerationDirectory!, $"{metadata.Name.ReplaceInvalidPathChars()}.osdb");

        await using FileStream osdbFileStream = File.OpenWrite(osdbFilePath);
        await using (BinaryWriter osdbVersionWriter = new(osdbFileStream, Encoding.UTF8, leaveOpen: true))
        {
            osdbVersionWriter.Write("o!dm8");
        }

        await using GZipStream compressionStream = new(osdbFileStream, CompressionMode.Compress);
        await using BinaryWriter osdbWriter = new(compressionStream);

        osdbWriter.Write("o!dm8");
        osdbWriter.Write(DateTime.Now.ToOADate());
        osdbWriter.Write(metadata.Uploader.Username);
        osdbWriter.Write(1); // Number of collections.
        osdbWriter.Write(metadata.Name);
        osdbWriter.Write(0); // OsuStats online id.
        osdbWriter.Write(downloadedBeatmaps.Count);

        foreach (Beatmap beatmap in downloadedBeatmaps)
        {
            osdbWriter.Write(beatmap.Id);
            osdbWriter.Write(beatmap.BeatmapsetId);
            osdbWriter.Write(beatmap.Beatmapset.Artist);
            osdbWriter.Write(beatmap.Beatmapset.Title);
            osdbWriter.Write(beatmap.Version); // Difficulty name.
            osdbWriter.Write(beatmap.Checksum);
            osdbWriter.Write(string.Empty); // Beatmapset's user comments.
            osdbWriter.Write((byte)beatmap.ModeInt);
            osdbWriter.Write(beatmap.DifficultyRating);
        }

        osdbWriter.Write(0); // Beatmaps that contain hashes only.
        osdbWriter.Write("By Piotrekol");
    }
}
