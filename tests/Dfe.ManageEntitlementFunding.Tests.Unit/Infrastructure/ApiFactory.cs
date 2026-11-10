using Dfe.ManageEntitlementFunding.Tests.Unit.Authentication;
using Dfe.ManageEntitlementFunding.Tests.Unit.Fakes;
using Dfe.ManageEntitlementFunding.Application.Providers.GetMyProviders;
using Dfe.ManageEntitlementFunding.Application.Providers.GetProvider;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Dfe.ManageEntitlementFunding.Tests.Unit.Infrastructure
{
    public sealed class ApiFactory : WebApplicationFactory<Program>
    {
        public FakeProviderQueryHandler Providers { get; } = new();

        protected override void ConfigureWebHost(
            IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services
                    .AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme =
                            TestAuthenticationHandler.AuthenticationScheme;

                        options.DefaultChallengeScheme =
                            TestAuthenticationHandler.AuthenticationScheme;
                    })
                    .AddScheme<AuthenticationSchemeOptions,
                        TestAuthenticationHandler>(
                            TestAuthenticationHandler.AuthenticationScheme,
                            _ => { });

                services.RemoveAll<IGetMyProvidersQueryHandler>();
                services.RemoveAll<IGetProviderQueryHandler>();

                services.AddSingleton<IGetMyProvidersQueryHandler>(
                    Providers);

                services.AddSingleton<IGetProviderQueryHandler>(
                    Providers);
            });
        }
    }
}
