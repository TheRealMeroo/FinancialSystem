using BuildingBlocks.Application.Errors;

namespace BuildingBlocks.Application.Results
{
    public class Result
    {
        public bool IsSuccess { get; }
        public IReadOnlyCollection<ApplicationError>? Errors { get; } = new List<ApplicationError>();

        public Result(bool isSuccess, IReadOnlyCollection<ApplicationError> errors)
        {
            if (isSuccess && errors != null && errors.Any())
                throw new InvalidOperationException("Successful result cannot have errors.");

            if (!isSuccess && (errors == null || !errors.Any()))
                throw new InvalidOperationException("Failure result must have an error.");

            IsSuccess = isSuccess;
            Errors = errors;
        }

        public static Result Success() => new Result(true, null);
        public static Result Failure(ApplicationError error) => new Result(false, new List<ApplicationError>() { error });
        public static Result Failure(List<ApplicationError> errors) => new Result(false, errors);
    }

    public class Result<T> : Result
    {
        public T? Value { get; }

        private Result(
            T? value,
            bool isSuccess,
            IReadOnlyCollection<ApplicationError> errors)
            : base(isSuccess, errors)
        {
            Value = value;
        }

        public static Result<T> Success(T value) => new Result<T>(value, true, null);
        public static Result<T> Failure(ApplicationError error) => new Result<T>(default, false, new List<ApplicationError>() { error });
        public static Result<T> Failure(List<ApplicationError> errors) => new Result<T>(default, false, errors);
    }
}
