namespace MercuryOMS.Application.Commons
{
    public class Result
    {
        public bool IsSuccess { get; init; }
        public string? ErrorMessage { get; init; }
        public static Result Success() => new Result { IsSuccess = true };
        public static Result Failure(string errorMessage) =>
            new Result { IsSuccess = false, ErrorMessage = errorMessage };
    }

    public class Result<T> : Result
    {
        public T? Value { get; init; }
        public static Result<T> Success(T value) =>
            new Result<T> { IsSuccess = true, Value = value };
        public new static Result<T> Failure(string errorMessage) =>
            new Result<T> { IsSuccess = false, ErrorMessage = errorMessage };
    }
}
