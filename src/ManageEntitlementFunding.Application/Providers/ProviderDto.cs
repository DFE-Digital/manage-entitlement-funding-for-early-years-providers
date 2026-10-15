namespace ManageEntitlementFunding.Application.Providers;

public record ProviderDto(
  Guid ProviderId,
  string ProviderName,
  string Urn);