using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Dfe.ManageEntitlementFunding.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
    public async Task GetReadiness_WithAvailableDatabase_ReturnsHealthy()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var baseFactory = new WebApplicationFactory<Program>();
        await using var factory = CreateFactory(baseFactory, connection);
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/health/ready");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("Healthy", await response.Content.ReadAsStringAsync());
    }

    [TestMethod]
    public async Task GetReadiness_WithUnavailableDatabase_ReturnsUnhealthyButLivenessIsHealthy()
    {
        await using var connection = new SqliteConnection(
            $"Data Source={Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.db")};Mode=ReadWrite");
        await using var baseFactory = new WebApplicationFactory<Program>();
        await using var factory = CreateFactory(baseFactory, connection);
        using var client = factory.CreateClient();

        using var readiness = await client.GetAsync("/health/ready");
        using var liveness = await client.GetAsync("/health");

        Assert.AreEqual(HttpStatusCode.ServiceUnavailable, readiness.StatusCode);
        Assert.AreEqual("Unhealthy", await readiness.Content.ReadAsStringAsync());
        Assert.AreEqual(HttpStatusCode.OK, liveness.StatusCode);
        Assert.AreEqual("Healthy", await liveness.Content.ReadAsStringAsync());
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

    private static WebApplicationFactory<Program> CreateFactory(
        WebApplicationFactory<Program> baseFactory, SqliteConnection? connection = null) =>
        baseFactory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configuration) =>
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["DevelopmentJwt:SigningKey"] = SigningKey
                }));
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<DbContextOptions<FundingDbContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<FundingDbContext>>();
                services.AddDbContext<FundingDbContext>(options =>
                {
                    if (connection is not null)
                    {
                        options.UseSqlite(connection);
                    }
                    else
                    {
                        options.UseSqlite("Data Source=:memory:");
                    }
                });
            });
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
