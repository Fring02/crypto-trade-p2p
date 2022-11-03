using System.Security.Claims;
using Api.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace Api.Authorization.Handlers;

public class AuthorizeCreateLotOwnerHandler : AuthorizationHandler<CreateLotOwnerRequirement, string>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CreateLotOwnerRequirement requirement, string resource)
    {
        foreach (var claim in context.User.Claims)
        {
            Console.WriteLine($"Claim {claim.Type}: {claim.Value}");
        }
        var userEmail = context.User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
        if (userEmail != resource) context.Fail(new AuthorizationFailureReason(this, "Custom failure occured"));
        else context.Succeed(requirement);
        return Task.CompletedTask;
    }
}