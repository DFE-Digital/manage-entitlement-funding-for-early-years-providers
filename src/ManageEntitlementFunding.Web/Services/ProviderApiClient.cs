using ManageEntitlementFunding.Domain.DataTransfer;

namespace ManageEntitlementFunding.Web.Services;

public class ProviderApiClient(HttpClient httpClient)
{
  private readonly HttpClient _httpClient = httpClient;

  public async Task<IEnumerable<ProviderSummaryDto>> GetUserProvidersAsync(CancellationToken ct = default)
  {
    try
    {
      var response = await _httpClient.GetFromJsonAsync<IEnumerable<ProviderSummaryDto>>("/api/providers/my-providers", ct);
      return response ?? [];
    }
    catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
    {
      return [];
    }
  }
}
