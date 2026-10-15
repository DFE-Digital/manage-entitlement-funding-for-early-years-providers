namespace ManageEntitlementFunding.Domain.DataTransfer;

public record ProviderSummaryDto(
  Guid ProviderId,
  string ProviderName,
  string Urn);