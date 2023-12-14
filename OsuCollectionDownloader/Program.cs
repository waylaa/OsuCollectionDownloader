using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OsuCollectionDownloader.Factories;
using OsuCollectionDownloader.Processors;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Diagnostics.CodeAnalysis;

namespace OsuCollectionDownloader;

internal sealed class Program
{
    [NotNull]
    internal static IServiceProvider? Services { get; private set; }

    private static async Task<int> Main(string[] args)
    {
        Option<int> id = new("--id", "The ID of a collection.");
        Option<string> extractionDirectory = new("--extraction-directory", "A directory where the downloaded beatmaps will be saved and extracted.");
        Option<string?> osdbGenerationDirectory = new
        (
            "--osdb-generation-directory",
            "A directory where a .osdb file will be generated in order to be used by Piotrekol's Collecton Manager. Leave empty if you want to disable the generation of such file."
        );
        Option<bool> downloadSequentially = new("--download-sequentially", getDefaultValue: () => false, "Whether or not to download the beatmaps concurrently.");

        RootCommand rootCommand = new("OsuCollectionDownloader")
        {
            id,
            extractionDirectory,
            osdbGenerationDirectory,
            downloadSequentially
        };

        rootCommand.SetHandler(async (ctx) =>
        {
            Console.Title = "OsuCollectionDownloader";

            DownloadProcessorBase processor = ctx.ParseResult.GetValueForOption(downloadSequentially)
                ? DownloadProcessorFactory.Create
                  (
                      new DownloadProcessorBaseOptions
                      (
                          ctx.ParseResult.GetValueForOption(id),
                          ctx.ParseResult.GetValueForOption(extractionDirectory)!,
                          ctx.ParseResult.GetValueForOption(osdbGenerationDirectory)
                      ),
                      Services.GetRequiredService<IHttpClientFactory>(),
                      Services.GetRequiredService<ILogger<SequentialDownloadProcessor>>()
                  )
                : DownloadProcessorFactory.Create
                  (
                      new DownloadProcessorBaseOptions
                      (
                          ctx.ParseResult.GetValueForOption(id),
                          ctx.ParseResult.GetValueForOption(extractionDirectory)!,
                          ctx.ParseResult.GetValueForOption(osdbGenerationDirectory)
                      ),
                      Services.GetRequiredService<IHttpClientFactory>(),
                      Services.GetRequiredService<ILogger<ConcurrentDownloadProcessor>>()
                  );

            await processor.DownloadAsync(ctx.GetCancellationToken());
        });

        return await new CommandLineBuilder(rootCommand)
            .UseDefaults()
            .AddMiddleware(middleware =>
            {
                Services = new ServiceCollection()
                    .AddLogging(loggingBuilder =>
                    {
                        loggingBuilder
                        .AddConsole()
                        .AddFilter("System.Net.Http.HttpClient.*.LogicalHandler", LogLevel.Warning)
                        .AddFilter("System.Net.Http.HttpClient.*.ClientHandler", LogLevel.Warning)
                        .SetMinimumLevel(LogLevel.Information);
                    })
                    .AddHttpClient("OsuCollectionDownloaderHttpClient", client =>
                    {
                        client.DefaultRequestHeaders.UserAgent.Add(new("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.45 Safari/537.36"));
                        client.DefaultRequestHeaders.Accept.Add(new("application/json"));
                        client.DefaultRequestHeaders.Accept.Add(new("application/octet-stream"));
                        client.Timeout = TimeSpan.FromSeconds(30);

                    }).Services
                    .AddMemoryCache()
                    .BuildServiceProvider();
            })
            .Build()
            .InvokeAsync(args);
    }
}
