// ManageEntitlementFunding.Api/Controllers/ProvidersController.cs
using System.Security.Claims;
using ManageEntitlementFunding.Api.Authentication;
using ManageEntitlementFunding.Application.Providers.GetMyProviders;
using ManageEntitlementFunding.Application.Providers.GetProvider;
using ManageEntitlementFunding.Domain.DataTransfer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEntitlementFunding.Api.Controllers;

[ApiController]
[Route("/api/providers")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Enforces JWT Bearer authentication across all actions in this controller
public class ProvidersController(
  IGetMyProvidersQueryHandler myProvidersQuery,
  IGetProviderQueryHandler providerQuery
) : ControllerBase
{
  private readonly IGetMyProvidersQueryHandler _myProvidersQuery = myProvidersQuery;
  private readonly IGetProviderQueryHandler _providerQuery = providerQuery;

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
  public async Task<IActionResult> GetMyProviders(CancellationToken cancellationToken)
  {
    var userId = User.GetUserId();

    if (userId is null)
    {
      return Unauthorized();
    }

    var query = new GetMyProvidersQuery(userId);

    var providers = await _myProvidersQuery.HandleAsync(query, cancellationToken);

    var providerSummaries = new List<ProviderSummaryDto>();

    foreach (var provider in providers)
    {
      providerSummaries.Add(new ProviderSummaryDto(provider.ProviderId, provider.ProviderName, provider.Urn));
    }

    return Ok(providerSummaries);
  }

  [HttpGet("{providerId:guid}")]
  [ProducesResponseType(typeof(ProviderSummaryDto), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  public async Task<IActionResult> GetProvider(
    Guid providerId,
    CancellationToken cancellationToken)
  {
    var userId = User.GetUserId();

    if (userId is null)
    {
      return Unauthorized();
    }

    var query = new GetProviderQuery(userId, providerId);

    var provider = await _providerQuery.HandleAsync(query, cancellationToken);

    if (provider is null)
    {
      return Forbid();
    }

    var providerSummary = new ProviderSummaryDto(provider.ProviderId, provider.ProviderName, provider.Urn);

    return Ok(providerSummary);
  }
}

