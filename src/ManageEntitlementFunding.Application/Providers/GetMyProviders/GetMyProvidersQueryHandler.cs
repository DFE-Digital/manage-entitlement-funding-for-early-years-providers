namespace ManageEntitlementFunding.Application.Providers.GetMyProviders;

public class GetMyProvidersQueryHandler : IGetMyProvidersQueryHandler
{
    public Task<IEnumerable<ProviderDetails>> HandleAsync(GetMyProvidersQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult(Array.Empty<ProviderDetails>().AsEnumerable());
    }
}