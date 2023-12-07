using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuCollectionDownloader.Cache;

internal sealed class FrozenDirectoryCache(string path)
{
    internal FrozenSet<string> Items { get; } = new HashSet<string>(Directory.EnumerateDirectories(path), StringComparer.OrdinalIgnoreCase).ToFrozenSet();
}
