namespace ManageEntitlementFunding.Application.Providers.GetProvider;

public interface IGetProviderQueryHandler
{
    Task<ProviderDto?> HandleAsync(
        GetProviderQuery query,
        CancellationToken cancellationToken);
}
