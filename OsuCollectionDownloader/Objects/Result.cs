namespace OsuCollectionDownloader.Objects;

internal readonly record struct Result<TValue>(TValue Value, Exception Error)
{
    internal readonly bool IsSuccessful => Error is null;

    internal readonly bool HasValue => Value is not null;

    public static implicit operator Result<TValue>(TValue value) => new(value, default!);

    public static implicit operator Result<TValue>(Exception error) => new(default!, error);
}