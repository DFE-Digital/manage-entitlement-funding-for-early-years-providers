extern alias Web;

using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using WebLoginModel = Web::Dfe.ManageEntitlementFunding.Web.Pages.DevLoginModel;

namespace Dfe.ManageEntitlementFunding.Tests.Unit.Authentication;

[TestClass]
public sealed class DevelopmentJwtConfigurationTests
{
    private const string SigningKey = "test-only-signing-key-with-at-least-32-characters";

    [TestMethod]
    [DataRow("Staging", SigningKey)]
    [DataRow("Production", SigningKey)]
    [DataRow("Production", "")]
    public void Api_WithDevelopmentKeyOutsideDevelopment_FailsStartup(string environment, string signingKey)
    {
        using var factory = new StartupFactory<Program>(environment, signingKey);

        var exception = Assert.ThrowsExactly<InvalidOperationException>(() => factory.CreateClient());

        StringAssert.Contains(exception.Message, "DevelopmentJwt:SigningKey must not be configured outside Development");
    }

    [TestMethod]
    [DataRow("Staging", SigningKey)]
    [DataRow("Production", SigningKey)]
    [DataRow("Production", "")]
    public void Web_WithDevelopmentKeyOutsideDevelopment_FailsStartup(string environment, string signingKey)
    {
        using var factory = new StartupFactory<WebLoginModel>(environment, signingKey);

        var exception = Assert.ThrowsExactly<InvalidOperationException>(() => factory.CreateClient());

        StringAssert.Contains(exception.Message, "DevelopmentJwt:SigningKey must not be configured outside Development");
    }

    [TestMethod]
    public async Task Development_WithSigningKey_StartsBothApps()
    {
        await using var api = new StartupFactory<Program>(Environments.Development, SigningKey);
        await using var web = new StartupFactory<WebLoginModel>(Environments.Development, SigningKey);
        using var apiClient = api.CreateClient();
        using var webClient = web.CreateClient();

        using var apiResponse = await apiClient.GetAsync("/openapi/v1.json");
        using var webResponse = await webClient.GetAsync("/DevLogin");

        Assert.AreEqual(HttpStatusCode.OK, apiResponse.StatusCode);
        Assert.AreEqual(HttpStatusCode.OK, webResponse.StatusCode);
    }

    [TestMethod]
    [DataRow("Staging")]
    [DataRow("Production")]
    public async Task NonDevelopment_WithoutDevelopmentKey_StartsBothApps(string environment)
    {
        await using var api = new StartupFactory<Program>(environment, null);
        await using var web = new StartupFactory<WebLoginModel>(environment, null);
        using var apiClient = api.CreateClient();
        using var webClient = web.CreateClient();

        using var apiResponse = await apiClient.GetAsync("/openapi/v1.json");
        using var webResponse = await webClient.GetAsync("/DevLogin");

        Assert.AreEqual(HttpStatusCode.NotFound, apiResponse.StatusCode);
        Assert.AreEqual(HttpStatusCode.NotFound, webResponse.StatusCode);
    }

    private sealed class StartupFactory<TEntryPoint>(string environment, string? signingKey)
        : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            var settings = new Dictionary<string, string?>
            {
                [HostDefaults.EnvironmentKey] = environment,
                ["DevelopmentBasicAuthEnabled"] = "false",
                ["DfESignIn:Authority"] = "https://identity.example.test",
                ["DfESignIn:ClientId"] = "test-client"
            };
            if (signingKey is not null)
            {
                settings["DevelopmentJwt:SigningKey"] = signingKey;
            }

            builder.ConfigureHostConfiguration(config => config.AddInMemoryCollection(settings));

            return base.CreateHost(builder);
        }
    }
}
