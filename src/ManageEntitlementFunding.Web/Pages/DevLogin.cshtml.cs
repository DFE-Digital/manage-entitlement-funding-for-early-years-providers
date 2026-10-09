using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageEntitlementFunding.Web.Pages;

[AllowAnonymous] // prevents the authorization middleware from redirecting /DevLogin back to itself
public class DevLoginModel : PageModel
{
  private readonly IWebHostEnvironment _env;

  public DevLoginModel(IWebHostEnvironment env)
  {
    _env = env;
  }

  [BindProperty]
  public string SubjectId { get; set; } = "user-123-test";

  [BindProperty]
  public string Name { get; set; } = "Jane Doe (Test User)";

  [BindProperty]
  public string Email { get; set; } = "jane.doe@example.gov.uk";

  public IActionResult OnGet()
  {
    // Block access if running outside Development environment
    if (!_env.IsDevelopment())
    {
      return NotFound();
    }

    return Page();
  }

  public async Task<IActionResult> OnPostAsync()
  {
    if (!_env.IsDevelopment())
    {
      return NotFound();
    }

    // Generate claims matching DfE Sign-In OIDC structure
    var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, SubjectId),
            new("sub", SubjectId),
            new(ClaimTypes.Name, Name),
            new(ClaimTypes.Email, Email)
        };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);

    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

    // Redirect to protected Dashboard (which checks provider link)
    return RedirectToPage("/Dashboard");
  }

  // Handles clearing the local auth cookie in Development
  public async Task<IActionResult> OnPostLogoutAsync()
  {
    if (!_env.IsDevelopment()) return NotFound();

    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    return RedirectToPage("/Index");
  }
}