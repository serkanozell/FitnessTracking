using System.Text.Json.Serialization;

namespace BuildingBlocks.Application.Results
{
    // Non-generic Result (void işlemler için)
    public class Result
    {
        public bool IsSuccess { get; }
        public Error? Error { get; }
        public bool IsFailure => !IsSuccess;

        protected Result(bool isSuccess, Error? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, null);
        public static Result Failure(Error error) => new(false, error);
    }

    public sealed class Result<T> : Result
    {
        public T? Data { get; }

        [JsonConstructor]
        private Result(bool isSuccess, T? data, Error? error) : base(isSuccess, error)
        {
            Data = data;
        }

        public static Result<T> Success(T data) => new(true, data, null);
        public new static Result<T> Failure(Error error) => new(false, default, error);

        // Implicit conversion
        public static implicit operator Result<T>(T value) => Success(value);
        public static implicit operator Result<T>(Error error) => Failure(error);
    }
}