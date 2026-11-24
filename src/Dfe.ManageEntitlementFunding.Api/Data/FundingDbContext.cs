using Microsoft.EntityFrameworkCore;

namespace Dfe.ManageEntitlementFunding.Api.Data;

public sealed class FundingDbContext(DbContextOptions<FundingDbContext> options) : DbContext(options);
