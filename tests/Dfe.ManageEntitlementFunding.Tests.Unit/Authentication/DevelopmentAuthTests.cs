extern alias Web;

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Web::Dfe.ManageEntitlementFunding.Web;

namespace Dfe.ManageEntitlementFunding.Tests.Unit.Authentication;

[TestClass]
public sealed class DevelopmentAuthTests
{
    private const string Password = "test-only-password:with-a-colon";

    [TestMethod]
    [DataRow("/")]
    [DataRow("/DevLogin")]
    [DataRow("/assets/styles.css")]
    [DataRow("/robots.txt")]
    [DataRow("/health")]
    public async Task Enabled_WithoutCredentials_ChallengesEveryPath(string path)
    {
        using var host = CreateHost();
        using var client = host.GetTestClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("AlwaysOn");

        using var response = await client.GetAsync(path);

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.AreEqual("Basic", response.Headers.WwwAuthenticate.Single().Scheme);
        Assert.IsTrue(response.Headers.CacheControl?.NoStore);
    }

    [TestMethod]
    [DataRow("Bearer token")]
    [DataRow("Basic not-base64!")]
    [DataRow("Basic dXNlcg==")]
    [DataRow("Basic dXNlcjo=")]
    [DataRow("Basic dXNlcjp3cm9uZw==")]
    public async Task Enabled_WithInvalidCredentials_Returns401(string authorization)
    {
        using var host = CreateHost();
        using var client = host.GetTestClient();
        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorization);

        using var response = await client.GetAsync("/");

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    [DataRow("developer", "Basic")]
    [DataRow("reviewer", "basic")]
    public async Task Enabled_WithCorrectPassword_AllowsRequest(string username, string scheme)
    {
        using var host = CreateHost();
        using var client = host.GetTestClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            scheme,
            Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{Password}")));

        using var response = await client.GetAsync("/DevLogin");

        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }

    [TestMethod]
    [DataRow("Development")]
    [DataRow("Production")]
    public async Task Disabled_WithoutPassword_LeavesRequestsUnchanged(string environment)
    {
        using var host = CreateHost(enabled: false, password: null, environment: environment);
        using var client = host.GetTestClient();

        using var response = await client.GetAsync("/");

        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        Assert.IsEmpty(response.Headers.WwwAuthenticate);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    public void Enabled_WithoutPassword_FailsStartup(string? password)
    {
        var exception = Assert.ThrowsExactly<InvalidOperationException>(
            () => CreateHost(password: password));

        StringAssert.Contains(exception.Message, "DevelopmentBasicAuthPassword");
    }

    [TestMethod]
    public void Enabled_OutsideDevelopment_FailsStartup()
    {
        var exception = Assert.ThrowsExactly<InvalidOperationException>(
            () => CreateHost(environment: Environments.Production));

        StringAssert.Contains(exception.Message, "only be enabled in Development");
    }

    [TestMethod]
    [DataRow("/DevLogin")]
    [DataRow("/assets/images/favicon.ico")]
    public async Task WebApplication_WithoutCredentials_ChallengesPagesAndAssets(string path)
    {
        await using var factory = new DevelopmentWebFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });

        using var response = await client.GetAsync(path);

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.AreEqual("Basic", response.Headers.WwwAuthenticate.Single().Scheme);
    }

    [TestMethod]
    public async Task WebApplication_WithCorrectPassword_PreservesSimulatedLogin()
    {
        await using var factory = new DevelopmentWebFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(Encoding.UTF8.GetBytes($"developer:{Password}")));

        using var response = await client.GetAsync("/DevLogin");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    private sealed class DevelopmentWebFactory : WebApplicationFactory<Web::Dfe.ManageEntitlementFunding.Web.Pages.DevLoginModel>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
                .UseEnvironment(Environments.Development)
                .ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["DevelopmentBasicAuthEnabled"] = "true",
                        ["DevelopmentBasicAuthPassword"] = Password
                    }));
        }
    }

    private static IHost CreateHost(
        bool enabled = true,
        string? password = Password,
        string environment = "Development")
    {
        return new HostBuilder()
            .UseEnvironment(environment)
            .ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["DevelopmentBasicAuthEnabled"] = enabled.ToString(),
                    ["DevelopmentBasicAuthPassword"] = password
                }))
            .ConfigureWebHost(web => web
                .UseTestServer()
                .Configure(app =>
                {
                    app.UseDevelopmentAuth();
                    app.Run(context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status204NoContent;
                        return Task.CompletedTask;
                    });
                }))
            .Start();
    }
}
