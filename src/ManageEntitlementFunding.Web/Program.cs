using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using GovUk.Frontend.AspNetCore;
using ManageEntitlementFunding.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.AccessDeniedPath = "/AccessDenied"; // holding page for unlinked user identities
})
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Authority = builder.Configuration["DfESignIn:Authority"];
    options.ClientId = builder.Configuration["DfESignIn:ClientId"];
    options.ClientSecret = builder.Configuration["DfESignIn:ClientSecret"];
    options.ResponseType = "code";

    // For consistency with other services that use DfE Sign-in
    options.CallbackPath = "/auth/cb";

    options.SaveTokens = true; // saves access_token in authentication session
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("organisation");
});

builder.Services.AddHttpClient<ProviderApiClient>(client =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7168";
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Add services to the container.   
builder.Services.AddRazorPages();

builder.Services.AddGovUkFrontend();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseGovUkFrontend();

app.UseStaticFiles();

// Map root favicon requests to the GDS asset location
app.MapGet("/fiavicon.ico", async context =>
{
    context.Response.Redirect("/assets/images/favicon.ico", permanent: true);
});

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
