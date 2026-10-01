using System.Net.Http.Json;
using EcePoc.Console.Models;

namespace EcePoc.Console.Services;

public class EligibilityCheckingClient(HttpClient httpClient)
{
  private readonly HttpClient _httpClient = httpClient;

  public Task<EligibilityData?> CheckWorkingFamiliesAsync(WorkingFamiliesData data, CancellationToken ct = default)
  {
    return ExecuteCheckAsync("/check/working-families", data, ct);
  }

  public Task<EligibilityData?> CheckTwoYearOfferAsync(TwoYearOfferData data, CancellationToken ct = default)
  {
    return ExecuteCheckAsync("/check/two-year-offer", data, ct);
  }

  // --- Generic asynchronous orchestrator ---
  private async Task<EligibilityData?> ExecuteCheckAsync<TRequest>(
      string endpoint,
      TRequest requestData,
      CancellationToken ct)
  {
    // Initiate check
    var postResponse = await _httpClient.PostAsJsonAsync(endpoint, new { Data = requestData }, ct);
    postResponse.EnsureSuccessStatusCode();

    var initialResult = await postResponse.Content.ReadFromJsonAsync<ApiEnvelope<CheckStatus>>(cancellationToken: ct);
    var status = initialResult?.Data?.Status;
    var statusUrl = initialResult?.Links?.GetEligibilityCheckStatus;
    var resultUrl = initialResult?.Links?.GetEligibilityCheck;

    // Poll until status changes from queued, or for 15 tries
    const int maxAttempts = 15;
    var attempts = 0;

    while (status == "queuedForProcessing" && !string.IsNullOrEmpty(statusUrl) && attempts < maxAttempts)
    {
      System.Console.WriteLine("Processing... polling for status update");

      await Task.Delay(TimeSpan.FromSeconds(1), ct);

      var statusResponse = await _httpClient.GetFromJsonAsync<ApiEnvelope<CheckStatus>>(statusUrl, ct);
      status = statusResponse?.Data?.Status;

      attempts++;
    }

    if (attempts >= maxAttempts)
    {
      System.Console.WriteLine("Warning: polling timed out waiting for API response");
      return null;
    }

    if (string.IsNullOrEmpty(resultUrl)) return null;

    // Fetch full final result
    var finalResponse = await _httpClient.GetFromJsonAsync<ApiEnvelope<EligibilityData>>(resultUrl, ct);
    return finalResponse?.Data;
  }
}