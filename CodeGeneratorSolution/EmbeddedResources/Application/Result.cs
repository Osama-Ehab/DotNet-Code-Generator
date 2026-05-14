
using {{TARGET_NAMESPACE}}.Core.Enums;

namespace {{TARGET_NAMESPACE}}.Application
{
    // DVLD.Common/Result.cs
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string ErrorMessage { get; }
        public ErrorType ErrorType { get; }

        // Protected constructor: Forces usage of static Factory methods
        protected Result(bool isSuccess, string errorMessage, ErrorType errorType)
        {
            // Guard Clause: Impossible state check
            if (isSuccess && errorMessage != string.Empty)
                throw new InvalidOperationException("A successful result cannot have an error message.");

            if (!isSuccess && errorMessage == string.Empty)
                throw new InvalidOperationException("A failure result must have an error message.");

            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            ErrorType = errorType;
        }

        // --- FACTORY METHODS (The Clean API) ---

        // 1. Success (No data)
        public static Result Success() => new Result(true, string.Empty, ErrorType.None);

        // 2. Failure (With categorization)
        public static Result Failure(string errorMessage, ErrorType type = ErrorType.Unexpected)
            => new Result(false, errorMessage, type);
    }
}