using System.Security.Claims;
using Api.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace Api.Authorization.Handlers;

public class RequireSameUserHandler : AuthorizationHandler<SameUserRequirement, Guid>
{
    private readonly ILogger<RequireSameUserHandler> _logger;
    public RequireSameUserHandler(ILogger<RequireSameUserHandler> logger)
    {
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserRequirement requirement, Guid resource)
    {
        _logger.LogInformation("Authenticated user claims: ");
        foreach (var claim in context.User.Claims) _logger.LogInformation("Claim {ClaimName}: {ClaimValue}", claim.Type, claim.Value);
        var authenticatedUserId = Guid.Parse(context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        _logger.LogInformation("User id: {UserId}", authenticatedUserId);
        if (authenticatedUserId == resource)
        {
            _logger.LogInformation("Requested resource: {Resource}: Authorization succeeded", resource);
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogError("Authorization failed: Requested resource is forbidden");
            context.Fail();
        }
        return Task.CompletedTask;
    }
}