using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OsuCollectionDownloader.Factories;
using OsuCollectionDownloader.Processors;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;

namespace OsuCollectionDownloader;

internal sealed class Program
{
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

            if (ctx.ParseResult.GetValueForOption(downloadSequentially))
            {
                SequentialDownloadProcessor processor = DownloadProcessorFactory.Create
                (
                    new DownloadProcessorBaseOptions
                    (
                        ctx.ParseResult.GetValueForOption(id),
                        ctx.ParseResult.GetValueForOption(extractionDirectory)!,
                        ctx.ParseResult.GetValueForOption(osdbGenerationDirectory)
                    ),
                    ctx.BindingContext.GetRequiredService<IHttpClientFactory>(),
                    ctx.BindingContext.GetRequiredService<ILogger<SequentialDownloadProcessor>>()
                );

                await processor.DownloadAsync(ctx.GetCancellationToken());
            }
            else
            {
                ConcurrentDownloadProcessor processor = DownloadProcessorFactory.Create
                (
                    new DownloadProcessorBaseOptions
                    (
                        ctx.ParseResult.GetValueForOption(id),
                        ctx.ParseResult.GetValueForOption(extractionDirectory)!,
                        ctx.ParseResult.GetValueForOption(osdbGenerationDirectory)
                    ),
                    ctx.BindingContext.GetRequiredService<IHttpClientFactory>(),
                    ctx.BindingContext.GetRequiredService<ILogger<ConcurrentDownloadProcessor>>()
                );

                await processor.DownloadAsync(ctx.GetCancellationToken());
            }
        });

        return await new CommandLineBuilder(rootCommand)
            .UseDefaults()
            .AddMiddleware(middleware =>
            {
                IServiceProvider provider = new ServiceCollection()
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
                    })
                    .Services.BuildServiceProvider();

                middleware.BindingContext.AddService(_ => provider.GetRequiredService<IHttpClientFactory>());

                if (middleware.ParseResult.GetValueForOption(downloadSequentially))
                {
                    middleware.BindingContext.AddService(_ => provider.GetRequiredService<ILogger<SequentialDownloadProcessor>>());
                }
                else
                {
                    middleware.BindingContext.AddService(_ => provider.GetRequiredService<ILogger<ConcurrentDownloadProcessor>>());
                }
            })
            .Build()
            .InvokeAsync(args);
    }
}
