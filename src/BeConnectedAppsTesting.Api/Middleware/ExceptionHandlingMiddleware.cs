namespace BeConnectedAppsTesting.Api.Middleware;

internal sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
#pragma warning disable CA1031
        catch (Exception ex)
#pragma warning restore CA1031
        {
            _logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                context.Request.Method,
                context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        var message = context.Response.StatusCode == StatusCodes.Status500InternalServerError
            ? "An internal server error occurred"
            : exception.Message;

        await context.Response.WriteAsJsonAsync(new { status = context.Response.StatusCode, error = message });
    }
}
