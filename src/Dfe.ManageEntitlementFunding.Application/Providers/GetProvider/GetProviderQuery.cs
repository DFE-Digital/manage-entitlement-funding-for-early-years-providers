namespace Dfe.ManageEntitlementFunding.Application.Providers.GetProvider;

public record GetProviderQuery(
    string UserId,
    Guid ProviderId
);
