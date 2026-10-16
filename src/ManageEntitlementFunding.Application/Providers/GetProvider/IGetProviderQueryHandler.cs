namespace ManageEntitlementFunding.Application.Providers.GetProvider;

public interface IGetProviderQueryHandler
{
    Task<ProviderDetails?> HandleAsync(
        GetProviderQuery query,
        CancellationToken cancellationToken);
}
