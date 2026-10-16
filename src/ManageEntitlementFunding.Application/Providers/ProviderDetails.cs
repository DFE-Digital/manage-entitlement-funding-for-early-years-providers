namespace ManageEntitlementFunding.Application.Providers;

public record ProviderDetails(
  Guid ProviderId,
  string ProviderName,
  string Urn);