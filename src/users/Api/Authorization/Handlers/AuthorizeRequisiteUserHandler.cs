using System.Security.Claims;
using Api.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using Shared.Dtos;
using Shared.Interfaces.Services;

namespace Api.Authorization.Handlers;

public class AuthorizeRequisiteUserHandler : AuthorizationHandler<RequisiteUserRequirement, long>
{
    private readonly IRequisiteService _requisiteService;
    private readonly ILogger<AuthorizeRequisiteUserHandler> _logger;
    public AuthorizeRequisiteUserHandler(IRequisiteService requisiteService, ILogger<AuthorizeRequisiteUserHandler> logger)
    {
        _requisiteService = requisiteService;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RequisiteUserRequirement requirement, long resource)
    {
        if (!Guid.TryParse(context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value, out var userId))
        {
            _logger.LogWarning("Authorization failed: id is not parsed");
            context.Fail();
            return;
        }
        var requisiteOwner = await _requisiteService.GetAsync<RequisiteUserDto>(r => r.UserId == userId && r.Id == resource);
        if (requisiteOwner is null)
        {
            _logger.LogError("Authorization failed: not found requisite owner (user) by id {UserId}", userId);
            context.Fail();
        }
        else
        {
            _logger.LogInformation("Authorization successful: Found requisite owner (user) by id {UserId}", userId);
            context.Succeed(requirement);
        }
    }
}