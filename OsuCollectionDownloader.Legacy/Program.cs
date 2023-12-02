using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OsuCollectionDownloader.Legacy.Processors;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;

namespace OsuCollectionDownloader.Legacy;

internal sealed class Program
{
    private static Task<int> Main(string[] args)
    {
        Option<int> id = new("--id", "The ID of a collection.");
        Option<DirectoryInfo> osuSongsDirectory = new("--osu-songs-directory", "A directory where the downloaded beatmaps will be saved and extracted.");
        Option<DirectoryInfo?> osdbFileDirectory = new
        (
            "--osdb-file-directory",
            "A directory where a .osdb file will be generated in order to be used by Piotrekol's Collecton Manager. Leave empty if you want to disable the generation of such file."
        );

        RootCommand rootCommand = new("OsuCollectionDownloader")
        {
            id,
            osuSongsDirectory,
            osdbFileDirectory,
        };

        rootCommand.SetHandler((ctx) =>
        {
            Console.Title = "OsuCollectionDownloader";

            DownloadProcessor downloader = new
            (
                new DownloadProcessorOptions
                (
                    ctx.ParseResult.GetValueForOption(id),
                    ctx.ParseResult.GetValueForOption(osuSongsDirectory)!,
                    ctx.ParseResult.GetValueForOption(osdbFileDirectory)
                ),
                ctx.BindingContext.GetRequiredService<IHttpClientFactory>(),
                ctx.BindingContext.GetRequiredService<ILogger<DownloadProcessor>>()
            );

            return downloader.DownloadAsync(ctx.GetCancellationToken());
        });

        return new CommandLineBuilder(rootCommand)
            .UseDefaults()
            .AddMiddleware(middleware =>
            {
                ServiceProvider provider = new ServiceCollection()
                    .AddLogging(loggingBuilder =>
                    {
                        loggingBuilder
                        .AddConsole()
#if DEBUG
                        .SetMinimumLevel(LogLevel.Debug);
#else
                        .AddFilter("System.Net.Http.HttpClient.*.LogicalHandler", LogLevel.Warning)
                        .AddFilter("System.Net.Http.HttpClient.*.ClientHandler", LogLevel.Warning)
                        .SetMinimumLevel(LogLevel.Information);
#endif
                    })
                    .AddHttpClient<DownloadProcessor>(client =>
                    {
                        client.DefaultRequestHeaders.Add
                        (
                            "User-Agent",
                            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.45 Safari/537.36"
                        );
                    })
                    .Services
                    .BuildServiceProvider();

                middleware.BindingContext.AddService(_ => provider.GetRequiredService<IHttpClientFactory>());
                middleware.BindingContext.AddService(_ => provider.GetRequiredService<ILogger<DownloadProcessor>>());
            })
            .Build()
            .InvokeAsync(args);
    }
}
