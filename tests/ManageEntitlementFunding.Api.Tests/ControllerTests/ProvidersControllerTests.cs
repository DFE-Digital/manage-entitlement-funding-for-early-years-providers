using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http.Json;
using ManageEntitlementFunding.Api.Tests.Infrastructure;
using ManageEntitlementFunding.Application.Providers;
using ManageEntitlementFunding.Contracts.Providers;

namespace ManageEntitlementFunding.Api.Tests.ControllerTests
{
    [TestClass]
    public sealed class ProvidersControllerTests
    {
        [TestMethod]
        public async Task GetMyProviders_WhenUnauthenticated_Returns401()
        {
            await using var factory = new ApiFactory();
            var client = factory.CreateClient();

            var response = await client.GetAsync(
                "/api/providers/my-providers");

            Assert.AreEqual(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        [TestMethod]
        public async Task GetMyProviders_WhenUserHasNoProviders_ReturnsEmptyList()
        {
            await using var factory = new ApiFactory();

            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Add(
                "X-Test-UserId",
                "test-user");

            var response = await client.GetAsync(
                "/api/providers/my-providers");

            response.EnsureSuccessStatusCode();

            var providers =
                await response.Content
                    .ReadFromJsonAsync<List<ProviderSummaryResponse>>();

            Assert.IsNotNull(providers);
            Assert.IsEmpty(providers);
        }

        [TestMethod]
        public async Task GetMyProviders_WhenUserHasProvider_ReturnsProvider()
        {
            await using var factory = new ApiFactory();

            var providerId = Guid.NewGuid();

            factory.Providers.ExistingProviders =
            [
                new ProviderDetails(providerId,
                                    "Test Nursery",
                                    "123456")
            ];

            var accessDictionary = new Dictionary<string, HashSet<Guid>>
            {
                { "test-user", new HashSet<Guid>{ providerId} }
            };

            factory.Providers.ProviderAccess = new ReadOnlyDictionary<string, HashSet<Guid>>(accessDictionary);

            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Add(
                "X-Test-UserId",
                "test-user");

            var response = await client.GetAsync(
                "/api/providers/my-providers");

            response.EnsureSuccessStatusCode();

            var providers =
                await response.Content
                    .ReadFromJsonAsync<List<ProviderSummaryResponse>>();

            Assert.HasCount(1, providers!);

            var provider = providers!.Single();

            Assert.AreEqual(providerId, provider.ProviderId);
            Assert.AreEqual("Test Nursery", provider.ProviderName);
            Assert.AreEqual("123456", provider.Urn);
        }

        [TestMethod]
        public async Task GetProvider_WhenUnauthenticated_Returns401()
        {
            await using var factory = new ApiFactory();
            var client = factory.CreateClient();

            var response = await client.GetAsync(
                $"/api/providers/{Guid.NewGuid()}");

            Assert.AreEqual(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        [TestMethod]
        public async Task GetProvider_WhenProviderDoesNotExist_Returns404()
        {
            await using var factory = new ApiFactory();

            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Add(
                "X-Test-UserId",
                "test-user");

            var response = await client.GetAsync(
                $"/api/providers/{Guid.NewGuid()}");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task GetProvider_WhenProviderExistsAndAccessible_ReturnsProvider()
        {
            await using var factory = new ApiFactory();

            var providerId = Guid.NewGuid();

            factory.Providers.ExistingProviders =
            [
                new ProviderDetails(providerId,
                                    "Test Nursery",
                                    "123456")
            ];

            var accessDictionary = new Dictionary<string, HashSet<Guid>>
            {
                { "test-user", new HashSet<Guid>{ providerId} }
            };

            factory.Providers.ProviderAccess = new ReadOnlyDictionary<string, HashSet<Guid>>(accessDictionary);

            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Add(
                "X-Test-UserId",
                "test-user");

            var response = await client.GetAsync(
                $"/api/providers/{providerId}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var provider = await client.GetFromJsonAsync<ProviderDetailsResponse?>($"/api/providers/{providerId}", new CancellationToken());

            Assert.IsNotNull(provider);
            Assert.AreEqual(providerId, provider.ProviderId);
            Assert.AreEqual("Test Nursery", provider.ProviderName);
            Assert.AreEqual("123456", provider.Urn);
        }

        [TestMethod]
        public async Task GetProvider_WhenProviderExistsButInaccessibleToUser_Returns404()
        {
            await using var factory = new ApiFactory();

            var providerId = Guid.NewGuid();

            factory.Providers.ExistingProviders =
            [
                new ProviderDetails(providerId,
                                    "Test Nursery",
                                    "123456")
            ];

            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Add(
                "X-Test-UserId",
                "test-user");

            var response = await client.GetAsync(
                $"/api/providers/{providerId}");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}