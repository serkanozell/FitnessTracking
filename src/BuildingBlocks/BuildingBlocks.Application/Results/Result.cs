using System.Text.Json.Serialization;

namespace BuildingBlocks.Application.Results;

[method: JsonConstructor]
public sealed class Result<T>(bool isSuccess, T? data, Error? error)
{
    public bool IsSuccess { get; } = isSuccess;
    public T? Data { get; } = data;
    public Error? Error { get; } = error;

    public static Result<T> Success(T data) => new(true, data, null);
    public static Result<T> Failure(Error error) => new(false, default, error);
}