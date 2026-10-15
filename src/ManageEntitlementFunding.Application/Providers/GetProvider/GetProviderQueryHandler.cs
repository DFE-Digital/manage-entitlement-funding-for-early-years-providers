namespace ManageEntitlementFunding.Application.Providers.GetProvider;

public class GetProviderQueryHandler : IGetProviderQueryHandler
{
    public Task<ProviderDto?> HandleAsync(GetProviderQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult<ProviderDto?>(null);
    }
}