namespace ManageEntitlementFunding.Application.Providers.GetMyProviders;

public interface IGetMyProvidersQueryHandler
{
    Task<IEnumerable<ProviderDetails>> HandleAsync(
        GetMyProvidersQuery query,
        CancellationToken cancellationToken);
}