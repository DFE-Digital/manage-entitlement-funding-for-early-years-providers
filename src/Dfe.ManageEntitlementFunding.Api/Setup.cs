using Dfe.ManageEntitlementFunding.Application.Providers.GetMyProviders;
using Dfe.ManageEntitlementFunding.Application.Providers.GetProvider;

internal static class Setup
{
    public static IServiceCollection SetUpApplication(this IServiceCollection services)
    {
        services.AddScoped<IGetMyProvidersQueryHandler, GetMyProvidersQueryHandler>();
        services.AddScoped<IGetProviderQueryHandler, GetProviderQueryHandler>();

        return services;
    }
}
