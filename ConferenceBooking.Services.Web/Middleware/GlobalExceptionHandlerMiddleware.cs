using System.Net;
using System.Text.Json;
using ConferenceBooking.Bll.Common.Shared.Exceptions;

namespace ConferenceBooking.Services.Web.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private const int Status499ClientClosedRequest = 499;
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            _logger.LogInformation("Запит було скасовано клієнтом.");
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = Status499ClientClosedRequest;
            }
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "Операцію було скасовано або минув час очікування.");
            if (!context.Response.HasStarted)
            {
                await WriteErrorResponseAsync(context, StatusCodes.Status408RequestTimeout, "Час очікування запиту вичерпано.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Необроблена помилка: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
            return;

        var (statusCode, message) = exception switch
        {
            RoomNotFoundException ex          => (HttpStatusCode.NotFound, ex.Message),
            BookingNotFoundException ex       => (HttpStatusCode.NotFound, ex.Message),
            BookingConflictException ex       => (HttpStatusCode.Conflict, ex.Message),
            RoomHasActiveBookingsException ex => (HttpStatusCode.Conflict, ex.Message),
            InvalidBookingTimeException ex    => (HttpStatusCode.BadRequest, ex.Message),
            _                                 => (HttpStatusCode.InternalServerError,
                                                   "Внутрішня помилка сервера. Спробуйте пізніше.")
        };

        await WriteErrorResponseAsync(context, (int)statusCode, message);
    }

    private static async Task WriteErrorResponseAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = statusCode,
            error = message,
            timestamp = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
