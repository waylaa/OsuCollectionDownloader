using System.Diagnostics.CodeAnalysis;

namespace OsuCollectionDownloader.Objects;

internal readonly record struct Result<TValue>(TValue? Value, Exception? Error)
{
    [MemberNotNullWhen(true, nameof(Value))]
    internal bool IsSucessfulWithValue => Value is not null && Error is null;

    public static implicit operator Result<TValue>(TValue value) => new(value, null);

    public static implicit operator Result<TValue>(Exception error) => new(default, error);
}