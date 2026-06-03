using System.Net;
using System.Text.Json;
using Serilog;

namespace ClearSpeechAI.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
            {
                Log.Error(ex, "Unhandled exception occurred after the response started");
                throw;
            }

            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, errorMessage) = MapException(exception);

        if (statusCode >= 500)
        {
            Log.Error(exception, "Unhandled exception occurred");
        }
        else
        {
            Log.Warning(exception, "Handled exception occurred");
        }

        context.Response.Clear();
        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = statusCode;

        var response = new
        {
            success = false,
            error   = errorMessage,
            detail  = exception.Message
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static (int StatusCode, string ErrorMessage) MapException(Exception exception)
    {
        return exception switch
        {
            HttpRequestException httpException when httpException.StatusCode == HttpStatusCode.Unauthorized =>
                ((int)HttpStatusCode.Unauthorized,
                    "Invalid OpenAI API key. Please verify your ApiKey in appsettings.json."),

            HttpRequestException httpException when httpException.StatusCode == HttpStatusCode.TooManyRequests =>
                ((int)HttpStatusCode.TooManyRequests,
                    "OpenAI rate limit reached. Please wait a moment and try again."),

            HttpRequestException httpException when httpException.StatusCode == HttpStatusCode.Forbidden =>
                ((int)HttpStatusCode.Forbidden,
                    "OpenAI request was forbidden. Please check your billing, permissions, or project settings."),

            HttpRequestException httpException when httpException.StatusCode == HttpStatusCode.PaymentRequired =>
                ((int)HttpStatusCode.PaymentRequired,
                    "OpenAI billing or quota is exhausted. Please check your account usage and payment settings."),

            _ => ((int)HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };
    }
}