namespace ManageEntitlementFunding.Contracts.Providers;

public record ProviderDetailsResponse(
  Guid ProviderId,
  string ProviderName,
  string Urn);