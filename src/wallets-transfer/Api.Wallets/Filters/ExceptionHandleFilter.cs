using Domain.Exceptions.Common;
using Domain.Exceptions.Wallets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Wallets.Api.Filters;

public class ExceptionHandleFilter : IExceptionFilter
{
    private readonly ILogger<ExceptionHandleFilter> _logger;
    public ExceptionHandleFilter(ILogger<ExceptionHandleFilter> logger) => _logger = logger;

    public void OnException(ExceptionContext context)
    {
        if(context.Exception is not ArgumentException && context.Exception is not EntityNotFoundException)
            _logger.LogError(context.Exception.Message);
        context.Result = context.Exception switch
        {
            ArgumentException e => new BadRequestObjectResult(e.Message),
            EntityNotFoundException => new NoContentResult(),
            AccountBalanceException e => new BadRequestObjectResult(e.Message),
            AccountLockedException e => new BadRequestObjectResult(e.Message),
            _ => new StatusCodeResult(500)
        };
    }
}