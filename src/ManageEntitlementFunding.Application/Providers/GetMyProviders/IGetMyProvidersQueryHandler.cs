namespace ManageEntitlementFunding.Application.Providers.GetMyProviders;

public interface IGetMyProvidersQueryHandler
{
    Task<IEnumerable<ProviderDto>> HandleAsync(
        GetMyProvidersQuery query,
        CancellationToken cancellationToken);
}