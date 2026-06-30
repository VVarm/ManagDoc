using Microsoft.AspNetCore.Http.HttpResults;

public class ExceptionHandlingMiddleware
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
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Ошибка при обработке запроса {Method} {Path}. Сообщение: {Message}",
                context.Request.Method,
                context.Request.Path,
                ex.Message);

            context.Response.StatusCode = ex switch
            {
                InvalidOperationException => StatusCodes.Status404NotFound,
                ArgumentException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
                // Распределить исключения на оперделённые логи или пусть будет, тот что я написала?
            };
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
    }
}