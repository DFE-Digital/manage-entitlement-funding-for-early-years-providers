using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ManageEntitlementFunding.Application.Providers.GetMyProviders;
using ManageEntitlementFunding.Application.Providers.GetProvider;
using ManageEntitlementFunding.Api.Tests.Authentication;
using ManageEntitlementFunding.Api.Tests.Fakes;

namespace ManageEntitlementFunding.Api.Tests.Infrastructure
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