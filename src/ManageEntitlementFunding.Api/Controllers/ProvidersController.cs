// ManageEntitlementFunding.Api/Controllers/ProvidersController.cs
using System.Security.Claims;
using ManageEntitlementFunding.Domain.DataTransfer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEntitlementFunding.Api.Controllers;

[ApiController]
[Route("api/providers")]
[Authorize] // Enforces JWT Bearer authentication across all actions in this controller
public class ProvidersController : ControllerBase
{
  /// <summary>
  /// Gets the providers associated with the currently authenticated user.
  /// </summary>
  /// <remarks>
  /// This is the only API endpoint which does not require the user to be mapped to at least one provider.
  /// For those users it will correctly return an empty enumerable.
  /// </remarks>
  [HttpGet("my-providers")]
  [ProducesResponseType(typeof(IEnumerable<ProviderSummaryDto>), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  public IActionResult GetMyProviders()
  {
    // Extract DfE Sign-In user identifier from claims
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
              ?? User.FindFirst("sub")?.Value;

    if (string.IsNullOrEmpty(userId))
    {
      return Unauthorized();
    }

    // STUB: Always returns an empty array until persistence logic is integrated
    var providers = Array.Empty<ProviderSummaryDto>();

    return Ok(providers);
  }
}

