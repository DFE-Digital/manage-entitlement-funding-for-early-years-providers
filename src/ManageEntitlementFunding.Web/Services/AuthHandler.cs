namespace ManageEntitlementFunding.Web.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

public class AuthHandler(
  IHttpContextAccessor httpContextAccessor,
  IConfiguration configuration,
  IWebHostEnvironment env) : DelegatingHandler
{
  private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
  private readonly IConfiguration _configuration = configuration;
  private readonly IWebHostEnvironment _env = env;

  protected override async Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    CancellationToken cancellationToken)
  {
    if (!_env.IsDevelopment())
    {
      // TODO: when OIDC is properly hooked up, this will need to read the token from session and set it as Bearer token on the request
      return await base.SendAsync(request, cancellationToken);
    }

    return await PropagateDevelopmentToken(request, cancellationToken);
  }

  private async Task<HttpResponseMessage> PropagateDevelopmentToken(
    HttpRequestMessage request,
     CancellationToken cancellationToken)
  {
    var httpContext = _httpContextAccessor.HttpContext;

    if (httpContext?.User.Identity?.IsAuthenticated == true)
    {
      var userId = httpContext.User.FindFirst("sub")?.Value
                ?? httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

      if (!string.IsNullOrEmpty(userId))
      {
        string signingKey = _configuration["DevelopmentJwt:SigningKey"]
              ?? throw new InvalidOperationException("Development JWT signing key is not configured.");

        var devToken = CreateDevelopmentJwt(userId, signingKey);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", devToken);
      }
    }

    return await base.SendAsync(request, cancellationToken);
  }

  private static string CreateDevelopmentJwt(
  string userId,
  string signingKey)
  {
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));

    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
      claims:
      [
        new Claim("sub", userId),
        new Claim(ClaimTypes.NameIdentifier, userId)
      ],
      expires: DateTime.UtcNow.AddHours(1),
      signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}