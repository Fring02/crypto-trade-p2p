using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Filters;

public class ExceptionHandleFilter : IExceptionFilter
{
    private readonly ILogger<ExceptionHandleFilter> _logger;
    public ExceptionHandleFilter(ILogger<ExceptionHandleFilter> logger) => _logger = logger;
    public void OnException(ExceptionContext context)
    {
        if(context.Exception is ArgumentException or SecurityTokenException) _logger.LogWarning(context.Exception.Message);
        else _logger.LogError(context.Exception.Message + "; Stack Trace: \n" + context.Exception.StackTrace);
        context.Result = context.Exception switch
        {
            ArgumentException e => new BadRequestObjectResult(e.Message),
            SecurityTokenException e => new UnauthorizedObjectResult(e.Message),
            _ => new StatusCodeResult(500)
        };
    }
}