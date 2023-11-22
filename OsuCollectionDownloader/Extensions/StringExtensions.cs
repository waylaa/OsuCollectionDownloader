using System.Runtime.CompilerServices;
using System.Text;

namespace OsuCollectionDownloader.Core.Extensions;

internal static class StringExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ReplaceInvalidPathChars(this string source)
        => new StringBuilder(source)
            .Replace(":", "_")
            .Replace("\"", "'")
            .Replace("<", string.Empty)
            .Replace(">", string.Empty)
            .Replace("/", "-")
            .Replace("\\", string.Empty)
            .Replace("|", string.Empty)
            .Replace("?", string.Empty)
            .Replace("*", string.Empty)
            .ToString();
}
