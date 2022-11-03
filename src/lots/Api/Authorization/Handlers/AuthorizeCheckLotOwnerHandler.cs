using System.Security.Claims;
using Api.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using Shared.Dtos;
using Shared.Interfaces.Repositories;

namespace Api.Authorization.Handlers;

public class AuthorizeCheckLotOwnerHandler : AuthorizationHandler<CheckLotOwnerRequirement, long>
{
    private readonly IServiceScopeFactory _factory;
    public AuthorizeCheckLotOwnerHandler(IServiceScopeFactory factory) => _factory = factory;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CheckLotOwnerRequirement requirement, long resource)
    {
        using var scope = _factory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ILotsRepository>();
        var userEmail = context.User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
        var lot = await repository.FindOneAndProjectAsync<LotOwnerDto>(l => l.Id == resource);
        if (lot!.OwnerEmail != userEmail) context.Fail();
        else context.Succeed(requirement);
    }
}