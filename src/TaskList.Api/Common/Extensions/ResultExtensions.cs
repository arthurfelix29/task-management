using System.Globalization;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Http.HttpResults;
using TaskList.Domain.Common;

namespace TaskList.Api.Common.Extensions;

public static class ResultExtensions
{
    public static ProblemHttpResult ToProblemDetails(this Result result)
    {
        Guard.Against.Null(result);

        if (result.IsSuccess)
            throw new InvalidOperationException("Cannot convert a successful result to ProblemDetails.");

        var statusCode = result.Error.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError,
        };

        return TypedResults.Problem(
            detail: result.Error.Description,
            statusCode: statusCode,
            title: result.Error.Code,
            type: ProblemTypeUriFor(statusCode));
    }

    public static string ProblemTypeUriFor(int statusCode)
        => string.Create(CultureInfo.InvariantCulture, $"https://datatracker.ietf.org/doc/html/rfc9110#name-{statusCode}");
}
