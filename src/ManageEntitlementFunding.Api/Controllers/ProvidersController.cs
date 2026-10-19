using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ManageEntitlementFunding.Api.Authentication;
using ManageEntitlementFunding.Application.Providers.GetMyProviders;
using ManageEntitlementFunding.Application.Providers.GetProvider;
using ManageEntitlementFunding.Contracts.Providers;

namespace ManageEntitlementFunding.Api.Controllers;

[ApiController]
[Route("/api/providers")]
[Authorize]
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
  [ProducesResponseType(typeof(IEnumerable<ProviderSummaryResponse>), StatusCodes.Status200OK)]
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

    var providerSummaries = new List<ProviderSummaryResponse>();

    foreach (var provider in providers)
    {
      providerSummaries.Add(new ProviderSummaryResponse(provider.ProviderId, provider.ProviderName, provider.Urn));
    }

    return Ok(providerSummaries);
  }

  [HttpGet("{providerId:guid}")]
  [ProducesResponseType(typeof(ProviderDetailsResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
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
      return NotFound();
    }

    var providerSummary = new ProviderDetailsResponse(provider.ProviderId, provider.ProviderName, provider.Urn);

    return Ok(providerSummary);
  }
}

