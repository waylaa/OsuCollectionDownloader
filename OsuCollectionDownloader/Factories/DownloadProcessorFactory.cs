using Microsoft.Extensions.Logging;
using OsuCollectionDownloader.Processors;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuCollectionDownloader.Factories;

internal sealed class DownloadProcessorFactory
{
    private static readonly FrozenDictionary<Type, Delegate> _constructors = new Dictionary<Type, Delegate>
    {
        {
            typeof(SequentialDownloadProcessor),
            new DownloadProcessorConstructor<SequentialDownloadProcessor>((options, clientFactory, logger) => new SequentialDownloadProcessor(options, clientFactory, logger))
        },
        {
            typeof(ConcurrentDownloadProcessor),
            new DownloadProcessorConstructor<ConcurrentDownloadProcessor>((options, clientFactory, logger) => new ConcurrentDownloadProcessor(options, clientFactory, logger))
        }

    }.ToFrozenDictionary();

    private delegate T DownloadProcessorConstructor<T>(DownloadProcessorBaseOptions options, IHttpClientFactory clientFactory, ILogger<T> logger);

    internal static T Create<T>(DownloadProcessorBaseOptions options, IHttpClientFactory clientFactory, ILogger<T> logger) where T : class
        => ((DownloadProcessorConstructor<T>)_constructors.GetValueRefOrNullRef(typeof(T)))(options, clientFactory, logger); // Casts the ref delegate and invokes it.
}
