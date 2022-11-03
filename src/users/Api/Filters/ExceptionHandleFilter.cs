using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters;

public class ExceptionHandleFilter : IExceptionFilter
{
    private readonly ILogger<ExceptionHandleFilter> _logger;
    public ExceptionHandleFilter(ILogger<ExceptionHandleFilter> logger) => _logger = logger;

    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case ArgumentException e:
                _logger.LogWarning("400 Bad Request: {e.Message}, Trace: \n {e.Trace}", e.Message, e.StackTrace);
                context.Result = new BadRequestObjectResult(e.Message);
                break;
            default:
                _logger.LogError("500 Server Error: {e.Message}, Trace: \n {e.Trace}", context.Exception.Message, context.Exception.StackTrace);
                context.Result = new StatusCodeResult(500);
                break;
        }
    }
}