using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TicketingApi.Validation;

public static partial class TicketValidator
{
    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
    private static partial Regex EmailRegex();

    public static List<ValidationResult> ValidateCreateRequest(
        string subject,
        string description,
        string customerName,
        string customerEmail)
    {
        var errors = new List<ValidationResult>();

        if (string.IsNullOrWhiteSpace(subject))
            errors.Add(new ValidationResult("Subject is required", new[] { nameof(subject) }));
        else if (subject.Length > 200)
            errors.Add(new ValidationResult("Subject cannot exceed 200 characters", new[] { nameof(subject) }));

        if (string.IsNullOrWhiteSpace(description))
            errors.Add(new ValidationResult("Description is required", new[] { nameof(description) }));
        else if (description.Length > 2000)
            errors.Add(new ValidationResult("Description cannot exceed 2000 characters", new[] { nameof(description) }));

        if (string.IsNullOrWhiteSpace(customerName))
            errors.Add(new ValidationResult("Customer name is required", new[] { nameof(customerName) }));
        else if (customerName.Length > 100)
            errors.Add(new ValidationResult("Customer name cannot exceed 100 characters", new[] { nameof(customerName) }));

        if (string.IsNullOrWhiteSpace(customerEmail))
            errors.Add(new ValidationResult("Customer email is required", new[] { nameof(customerEmail) }));
        else if (!EmailRegex().IsMatch(customerEmail))
            errors.Add(new ValidationResult("Customer email is not valid", new[] { nameof(customerEmail) }));

        return errors;
    }

    public static List<ValidationResult> ValidateUpdateRequest(
        string? subject,
        string? description,
        string? assignedTo)
    {
        var errors = new List<ValidationResult>();

        if (subject is not null && subject.Length > 200)
            errors.Add(new ValidationResult("Subject cannot exceed 200 characters", new[] { nameof(subject) }));

        if (description is not null && description.Length > 2000)
            errors.Add(new ValidationResult("Description cannot exceed 2000 characters", new[] { nameof(description) }));

        if (assignedTo is not null && assignedTo.Length > 100)
            errors.Add(new ValidationResult("Assigned to cannot exceed 100 characters", new[] { nameof(assignedTo) }));

        return errors;
    }
}
