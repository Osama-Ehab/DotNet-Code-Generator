
using CodeGeneratorSolution.Templetes.Infrastructure.Enums;

namespace CodeGeneratorSolution.EmbeddedResources.Application
{
    // DVLD.Common/ResultT.cs
    public class Result<T> : Result
    {
        public T Value { get; }

        // Private constructor
        private Result(T value, bool isSuccess, string error, ErrorType errorType)
            : base(isSuccess, error, errorType)
        {
            Value = value;
        }

        // --- FACTORY METHODS ---

        public static Result<T> Success(T value)
            => new Result<T>(value, true, string.Empty, ErrorType.None);

        // Hides the base Failure to return Result<T> instead of Result
        public new static Result<T> Failure(string error, ErrorType type = ErrorType.Unexpected)
            => new Result<T>(default, false, error, type);
    }
}