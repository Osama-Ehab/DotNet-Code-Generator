
using CodeGeneratorSolution.Templetes.Infrastructure.Enums;

namespace CodeGeneratorSolution.EmbeddedResources.Application
{
    // DVLD.Common/Result.cs
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string Error { get; }
        public ErrorType ErrorType { get; }

        // Protected constructor: Forces usage of static Factory methods
        protected Result(bool isSuccess, string error, ErrorType errorType)
        {
            // Guard Clause: Impossible state check
            if (isSuccess && error != string.Empty)
                throw new InvalidOperationException("A successful result cannot have an error message.");

            if (!isSuccess && error == string.Empty)
                throw new InvalidOperationException("A failure result must have an error message.");

            IsSuccess = isSuccess;
            Error = error;
            ErrorType = errorType;
        }

        // --- FACTORY METHODS (The Clean API) ---

        // 1. Success (No data)
        public static Result Success() => new Result(true, string.Empty, ErrorType.None);

        // 2. Failure (With categorization)
        public static Result Failure(string error, ErrorType type = ErrorType.Unexpected)
            => new Result(false, error, type);
    }
}