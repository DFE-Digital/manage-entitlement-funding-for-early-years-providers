using ManageEntitlementFunding.Contracts.Providers;

namespace ManageEntitlementFunding.Web.Services;

public class ProviderApiClient(HttpClient httpClient)
{
  private readonly HttpClient _httpClient = httpClient;

  public async Task<IEnumerable<ProviderDetailsResponse>> GetUserProvidersAsync(
    CancellationToken cancellationToken = default)
  {
    var response = await _httpClient.GetFromJsonAsync<IEnumerable<ProviderDetailsResponse>>("/api/providers/my-providers", cancellationToken);
    return response ?? [];
  }

  public async Task<ProviderDetailsResponse?> GetProviderAsync(
    Guid providerId,
    CancellationToken cancellationToken = default)
  {
    return await _httpClient.GetFromJsonAsync<ProviderDetailsResponse?>($"/api/providers/{providerId}", cancellationToken);
  }
}
