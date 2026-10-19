namespace ManageEntitlementFunding.Contracts.Providers;

public record ProviderSummaryResponse(
  Guid ProviderId,
  string ProviderName,
  string Urn);