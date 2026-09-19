using FluentValidation.Results;
using Shared;

namespace DevQuestions.Application.Extensions;

public static class ValidationExtensions
{
    public static Failure ToErrors(this ValidationResult validationResult)
    {
        return validationResult.Errors.Select(failure => Error.Validation(failure.ErrorCode, failure.ErrorMessage, failure.PropertyName)).ToArray();
    }
}
