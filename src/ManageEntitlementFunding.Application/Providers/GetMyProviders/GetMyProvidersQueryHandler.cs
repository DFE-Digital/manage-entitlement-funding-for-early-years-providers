namespace ManageEntitlementFunding.Application.Providers.GetMyProviders;

public class GetMyProvidersQueryHandler : IGetMyProvidersQueryHandler
{
    public Task<IEnumerable<ProviderDto>> HandleAsync(GetMyProvidersQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult(Array.Empty<ProviderDto>().AsEnumerable());
    }
}