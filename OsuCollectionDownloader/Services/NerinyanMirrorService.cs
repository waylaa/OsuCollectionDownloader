﻿using OsuCollectionDownloader.Objects;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace OsuCollectionDownloader.Services;

internal class NerinyanMirrorService(HttpClient client) : IMirrorService
{
    public string BaseApiUrl => "https://api.nerinyan.moe";

    public async Task<Result<bool>> DownloadAsync(string filePath, int beatmapSetId, CancellationToken token)
    {
        try
        {
            using HttpResponseMessage response = await client.GetAsync($"{BaseApiUrl}/d/{beatmapSetId}", HttpCompletionOption.ResponseHeadersRead, token);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            await using FileStream beatmapStream = File.OpenWrite(filePath);
            await response.Content.CopyToAsync(beatmapStream, token);

            return true;
        }
        catch (Exception ex) when (ex is IOException or HttpRequestException)
        {
            return ex;
        }
    }

    public async Task<Result<JsonDocument?>> SearchAsync(string query, CancellationToken token)
    {
        var filterRange = new { min = 0, max = 0 };

        JsonObject searchPayload = new()
        {
            ["extra"] = string.Empty,
            ["ranked"] = "all",
            ["nsfw"] = true,
            ["option"] = string.Empty,
            ["m"] = string.Empty,
            ["totalLength"] = JsonSerializer.SerializeToNode(filterRange),
            ["maxCombo"] = JsonSerializer.SerializeToNode(filterRange),
            ["difficultyRating"] = JsonSerializer.SerializeToNode(filterRange),
            ["accuracy"] = JsonSerializer.SerializeToNode(filterRange),
            ["ar"] = JsonSerializer.SerializeToNode(filterRange),
            ["cs"] = JsonSerializer.SerializeToNode(filterRange),
            ["drain"] = JsonSerializer.SerializeToNode(filterRange),
            ["bpm"] = JsonSerializer.SerializeToNode(filterRange),
            ["sort"] = "title_desc",
            ["page"] = 0,
            ["query"] = query
        };

        byte[] searchPayloadData = Encoding.UTF8.GetBytes(searchPayload.ToJsonString());
        string encodedSearchPayloadData = Convert.ToBase64String(searchPayloadData);

        try
        {
            return await client.GetFromJsonAsync<JsonDocument>($"{BaseApiUrl}/search?b64={encodedSearchPayloadData}&ps=60", token);
        }
        catch (HttpRequestException ex)
        {
            return ex;
        }
    }
}
