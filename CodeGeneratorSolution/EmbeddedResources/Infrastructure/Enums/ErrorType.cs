
namespace CodeGeneratorSolution.Templetes.Infrastructure.Enums
{
    public enum ErrorType
    {
        None = 0,
        Validation = 1,       // Input is invalid (e.g., empty string)
        NotFound = 2,         // Record not found
        Conflict = 3,         // Duplicate Unique Index (e.g., NationalNo exists)
        ConstraintViolation = 4, // Foreign Key / Check Constraint (e.g., Delete user who has licenses)
        Unauthorized = 5,     // User doesn't have permission
        Database = 6,         // System failures (Timeout, Connection)
        Unexpected = 7        // Crashes
    }
}