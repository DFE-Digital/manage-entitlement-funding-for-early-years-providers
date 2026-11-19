using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Dfe.ManageEntitlementFunding.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsDevelopment() && builder.Configuration["DevelopmentJwt:SigningKey"] is not null)
{
    throw new InvalidOperationException("DevelopmentJwt:SigningKey must not be configured outside Development.");
}

builder.Services.AddHttpContextAccessor();

// Prevent default claim remapping
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        var signingKey = builder.Configuration["DevelopmentJwt:SigningKey"]
                ?? throw new InvalidOperationException("Development JWT signing key not configured");

        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
            };
    }
    else
    {
        options.Authority = builder.Configuration["DfESignIn:Authority"];
        options.Audience = builder.Configuration["DfESignIn:ClientId"];
        options.RequireHttpsMetadata = true;
    }
});

builder.Services.SetUpApplication();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<FundingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Funding")
        ?? throw new InvalidOperationException("ConnectionStrings:Funding must be configured.")));
builder.Services.AddHealthChecks()
    .AddDbContextCheck<FundingDbContext>("database", tags: ["ready"]);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => false
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("ready")
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();

public partial class Program;
