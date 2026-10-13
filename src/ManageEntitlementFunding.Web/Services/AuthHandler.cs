namespace ManageEntitlementFunding.Web.Services;

using System.Net.Http.Headers;
using System.Security.Claims;

public class AuthHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
  private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

  protected override async Task<HttpResponseMessage> SendAsync(
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
        var devToken = CreateMockJwt(userId);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", devToken);
      }
    }

    return await base.SendAsync(request, cancellationToken);
  }

  private static string CreateMockJwt(string userId)
  {
    // {"alg":"none","typ":"JWT"}
    const string header = "eyJhbGciOiJub25lIiwidHlwIjoiSldUIn0";

    var payloadJson = $"{{\"sub\":\"{userId}\",\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier\":\"{userId}\"}}";

    var payload = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(payloadJson))
        .TrimEnd('=')
        .Replace('+', '-')
        .Replace('/', '_');

    return $"{header}.{payload}.";
  }
}