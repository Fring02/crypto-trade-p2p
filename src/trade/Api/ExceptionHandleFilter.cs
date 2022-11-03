using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api;

public class ExceptionHandleFilter : IExceptionFilter
{
    private readonly ILogger<ExceptionHandleFilter> _logger;
    public ExceptionHandleFilter(ILogger<ExceptionHandleFilter> logger) => _logger = logger;
    public void OnException(ExceptionContext context)
    {
        if(context.Exception is ArgumentException) _logger.LogWarning(context.Exception.Message);
        else _logger.LogError(context.Exception.Message + "; Stack Trace: \n" + context.Exception.StackTrace);
        context.Result = context.Exception switch
        {
            ArgumentException e => new BadRequestObjectResult(e.Message),
            SessionNotFoundException => new NotFoundObjectResult("Session not found"),
            SessionBlockedException e => new UnauthorizedObjectResult(e.Message),
            HttpRequestException => new StatusCodeResult(500),
            _ => new StatusCodeResult(500)
        };
    }
}