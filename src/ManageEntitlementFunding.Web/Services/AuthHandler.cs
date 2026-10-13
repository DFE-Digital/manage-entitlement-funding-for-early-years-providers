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
      return await base.SendAsync(request, cancellationToken);
    }

    return await PropagateDevelopmentToken(request, cancellationToken);
  }

  private async Task<HttpResponseMessage> PropagateDevelopmentToken(HttpRequestMessage request, CancellationToken cancellationToken)
  {
    var httpContext = _httpContextAccessor.HttpContext;

    if (httpContext?.User.Identity?.IsAuthenticated == true)
    {
      var userId = httpContext.User.FindFirst("sub")?.Value
                ?? httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

      if (!string.IsNullOrEmpty(userId))
      {
        var devToken = CreateDevelopmentJwt(
          userId,
          _configuration["DevelopmentJwt:SigningKey"]!);

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