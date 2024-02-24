#region

using PlanningPoker.Domain.Common.Extensions;

#endregion

namespace PlanningPoker.Domain.Validation;

public record Error
{
    public Error(string? root, string code, string message)
    {
        Code = root.IsPresent() ? $"{root}.{code}" : code;
        Message = message;
    }

    public Error(string code, string message) : this(null, code, message)
    {
    }

    public string Code { get; private set; }
    public string Message { get; private set; }

    public static Error NullOrEmpty(string root = "", string code = "")
    {
        return new Error(root, code, "Provided value cannot be null, empty or white space.");
    }

    public static Error InvalidEmail(string root = "", string code = "email")
    {
        return new Error(root, code, "Provided email is not valid.");
    }

    public static Error GreaterThan(string root = "", string code = "value", int value = 0)
    {
        return new Error(root, code, $"Provided value must be greater than {value}.");
    }

    public static Error MinLength(string root = "", string code = "string", int minLength = 0)
    {
        return new Error(root, code,
            $"The provided string does not meet the minimum length requirement. Min length: {minLength}.");
    }

    public static Error EmptyCollection(string root = "", string code = "")
    {
        return new Error(root, code, "The list cannot be empty.");
    }
}