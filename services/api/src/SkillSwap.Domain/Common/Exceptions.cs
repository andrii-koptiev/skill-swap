namespace SkillSwap.Domain.Common;

/// <summary>
/// Exception thrown when a domain business rule is violated.
/// This represents expected business logic failures that should result in user-friendly error messages.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message)
        : base(message) { }

    public DomainException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when validation fails for domain entities or value objects.
/// This represents data validation failures that should be presented to the user.
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(string message)
        : base(message) { }

    public ValidationException(string message, Exception innerException)
        : base(message, innerException) { }

    /// <summary>
    /// Creates a validation exception with multiple validation errors.
    /// </summary>
    /// <param name="errors">Dictionary of field names and their validation errors.</param>
    public ValidationException(Dictionary<string, string[]> errors)
        : base("One or more validation failures occurred.")
    {
        Errors = errors;
    }

    /// <summary>
    /// Validation errors grouped by field name.
    /// </summary>
    public Dictionary<string, string[]> Errors { get; } = new();
}
