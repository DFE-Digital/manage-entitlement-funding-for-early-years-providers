using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["DfESignIn:Authority"];
    options.Audience = builder.Configuration["DfESignIn:ClientId"];
    options.RequireHttpsMetadata = true;
});

// Custom authorization handler
builder.Services.AddScoped<IAuthorizationHandler, ProviderAssociationHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireLinkedProvider", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new ProviderAssociationRequirement());
    });
});

// Set up controller support
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Secure endpoint mapping
app.MapGet("/api/entitlements", [Authorize(Policy = "RequireLinkedProvider")] () =>
{
    return Results.Ok(new[] { "Working Families 2YO", "Universal 3YO" });
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();
