using ClinicBooking.Application.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ClinicBooking.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception,
            "Loi khong xu ly duoc: {Message}",
            exception.Message);

        var (statusCode, title, detail, errors) = MapException(exception);

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = $"https://httpstatuses.io/{statusCode}",
            Instance = httpContext.Request.Path
        };

        if (errors is not null)
        {
            problemDetails.Extensions["errors"] = errors;
        }

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        });
    }

    private static (int StatusCode, string Title, string Detail, object? Errors)
        MapException(Exception exception) => exception switch
    {
        ValidationException ex => (
            StatusCodes.Status400BadRequest,
            "Du lieu khong hop le",
            "Mot hoac nhieu truong khong thoa man dieu kien xac thuc.",
            ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray())),

        NotFoundException ex => (
            StatusCodes.Status404NotFound,
            "Khong tim thay tai nguyen",
            ex.Message,
            null),

        ConflictException ex => (
            StatusCodes.Status409Conflict,
            "Xung dot du lieu",
            ex.Message,
            null),

        ForbiddenException ex => (
            StatusCodes.Status403Forbidden,
            "Khong co quyen truy cap",
            ex.Message,
            null),

        UnauthorizedAccessException ex => (
            StatusCodes.Status401Unauthorized,
            "Chua xac thuc",
            ex.Message,
            null),

        _ => (
            StatusCodes.Status500InternalServerError,
            "Loi he thong",
            "Da xay ra loi khong mong muon. Vui long thu lai sau.",
            null)
    };
}
