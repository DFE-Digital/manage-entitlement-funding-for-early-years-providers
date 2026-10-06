using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

public class ProviderAssociationRequirement : IAuthorizationRequirement { }

public class ProviderAssociationHandler : AuthorizationHandler<ProviderAssociationRequirement>
{
  protected override Task HandleRequirementAsync(
    AuthorizationHandlerContext context,
    ProviderAssociationRequirement requirement)
  {
    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value     ?? context.User.FindFirst("sub")?.Value;

    if (string.IsNullOrEmpty(userId))
    {
      context.Fail();
      return Task.CompletedTask;
    }

    // Tracer bullet stub: always return false until provider database links are built
    bool isAssociatedWithProvider = CheckUserProviderAssociationStub(userId);

    if (isAssociatedWithProvider)
    {
      context.Succeed(requirement);
    }
    else
    {
      // Fails evaluation: API will return 403 forbidden without leaking state
      context.Fail();
    }

    return Task.CompletedTask;
  }

  private static bool CheckUserProviderAssociationStub(string userId)
  {
    return false;
  }
}