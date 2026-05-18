using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TaskList.Api.Common.Extensions;

namespace TaskList.Api.Common.ExceptionHandling;

public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailsService, IHostEnvironment env, ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        Guard.Against.Null(httpContext);
        Guard.Against.Null(exception);

        logger.LogError(
            exception,
            "Unhandled exception while processing {Method} {Path}",
            httpContext.Request.Method,
            httpContext.Request.Path);

        var (statusCode, title) = exception switch
        {
            ArgumentException => (StatusCodes.Status400BadRequest, "Bad request"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
            UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "Forbidden"),
            _ => (StatusCodes.Status500InternalServerError, "Internal server error"),
        };

        var productionDetail = statusCode < StatusCodes.Status500InternalServerError ? title : "An unexpected error occurred.";
        var detail = env.IsDevelopment() ? exception.Message : productionDetail;

        httpContext.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Type = ResultExtensions.ProblemTypeUriFor(statusCode),
                Detail = detail,
            },
        });
    }
}
