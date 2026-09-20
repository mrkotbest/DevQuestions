using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace DevQuestions.Presenters.ResponseExtensions;

public static class ResponseExtensions
{
    public static ActionResult ToResponse(this Failure failure)
    {
        if (!failure.Any())
        {
            return new ObjectResult(null)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        var distinctErrorTypes = failure
            .Select(e => e.Type)
            .Distinct()
            .ToList();

        int statusCode = distinctErrorTypes.Count == 1
            ? GetStatusCodeFromErrorType(distinctErrorTypes.First())
            : StatusCodes.Status500InternalServerError;

        return new ObjectResult(failure)
        {
            StatusCode = statusCode
        };
    }

    private static int GetStatusCodeFromErrorType(Errortype errorType)
    {
        return errorType switch
        {
            Errortype.Validation => StatusCodes.Status400BadRequest,
            Errortype.NotFound => StatusCodes.Status404NotFound,
            Errortype.Unauthorized => StatusCodes.Status401Unauthorized,
            Errortype.Forbidden => StatusCodes.Status403Forbidden,
            Errortype.Conflict => StatusCodes.Status409Conflict,
            Errortype.InternalServerError => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}
