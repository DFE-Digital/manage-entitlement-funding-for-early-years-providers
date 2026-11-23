using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Dfe.ManageEntitlementFunding.Api.Integration.Tests;

[TestClass]
public sealed class ApiIntegrationTests
{
    private const string SigningKey = "integration-test-signing-key-with-at-least-32-bytes";

    [TestMethod]
    public async Task GetHealth_ReturnsSuccessAndHealthyStatus()
    {
        await using var baseFactory = new WebApplicationFactory<Program>();
        await using var factory = CreateFactory(baseFactory);
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/health");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("Healthy", await response.Content.ReadAsStringAsync());
    }

    [TestMethod]
    public async Task GetMyProviders_WithValidToken_ReturnsSuccessAndEmptyList()
    {
        await using var baseFactory = new WebApplicationFactory<Program>();
        await using var factory = CreateFactory(baseFactory);

        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", CreateAccessToken());

        using var response = await client.GetAsync("/api/providers/my-providers");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var providers = await response.Content.ReadFromJsonAsync<List<JsonElement>>();

        Assert.IsNotNull(providers);
        Assert.IsEmpty(providers);
    }

    private static WebApplicationFactory<Program> CreateFactory(WebApplicationFactory<Program> baseFactory) =>
        baseFactory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configuration) =>
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["DevelopmentJwt:SigningKey"] = SigningKey
                }));
        });

    private static string CreateAccessToken()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            claims: [new Claim("sub", "integration-test-user")],
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
