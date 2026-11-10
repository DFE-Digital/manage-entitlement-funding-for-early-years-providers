using System.Collections.ObjectModel;
using Dfe.ManageEntitlementFunding.Application.Providers;
using Dfe.ManageEntitlementFunding.Application.Providers.GetMyProviders;
using Dfe.ManageEntitlementFunding.Application.Providers.GetProvider;

namespace Dfe.ManageEntitlementFunding.Tests.Unit.Fakes
{
    public sealed class FakeProviderQueryHandler :
        IGetMyProvidersQueryHandler,
        IGetProviderQueryHandler
    {
        public IReadOnlyCollection<ProviderDetails> ExistingProviders { get; set; } = [];

        public IReadOnlyDictionary<string, HashSet<Guid>> ProviderAccess { get; set; }
         = new ReadOnlyDictionary<string, HashSet<Guid>>(new Dictionary<string, HashSet<Guid>>());

        public Task<IEnumerable<ProviderDetails>> HandleAsync(
            GetMyProvidersQuery query,
            CancellationToken cancellationToken)
        {
            if (!ProviderAccess.TryGetValue(query.UserId, out var accessibleIds))
            {
                return Task.FromResult(Enumerable.Empty<ProviderDetails>());
            }

            var providers = ExistingProviders
                        .Where(p => accessibleIds.Contains(p.ProviderId));

            return Task.FromResult(providers);
        }

        public Task<ProviderDetails?> HandleAsync(
            GetProviderQuery query,
            CancellationToken cancellationToken)
        {
            if (!ProviderAccess.TryGetValue(query.UserId, out var accessibleIds) || !accessibleIds.Contains(query.ProviderId))
            {
                return Task.FromResult<ProviderDetails?>(null);
            }

            var provider = ExistingProviders.SingleOrDefault(
                x => x.ProviderId == query.ProviderId);

            return Task.FromResult(provider);
        }
    }
}
