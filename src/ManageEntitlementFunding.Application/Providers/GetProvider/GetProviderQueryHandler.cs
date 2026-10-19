namespace ManageEntitlementFunding.Application.Providers.GetProvider;

public class GetProviderQueryHandler : IGetProviderQueryHandler
{
    public Task<ProviderDetails?> HandleAsync(GetProviderQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult<ProviderDetails?>(null);
    }
}