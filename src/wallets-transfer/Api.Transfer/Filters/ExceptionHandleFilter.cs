using Domain.Exceptions.Common;
using Domain.Exceptions.Wallets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Transfer.Filters;

public class ExceptionHandleFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        context.Result = context.Exception switch
        {
            ArgumentException e => new BadRequestObjectResult(e.Message),
            AccountBalanceException e => new BadRequestObjectResult(e.Message),
            EntityNotFoundException => new NoContentResult(),
            AccountLockedException e => new BadRequestObjectResult(e.Message),
            _ => new StatusCodeResult(500)
        };
    }
}