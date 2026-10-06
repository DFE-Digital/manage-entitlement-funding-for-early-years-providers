using System.Security.Claims;
using ManageEntitlementFunding.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageEntitlementFunding.Web.Pages;

[Authorize] // Enforces authentication before loading this page
public class DashboardModel(ProviderApiClient providerApiClient) : PageModel
{
  private readonly ProviderApiClient _providerApiClient = providerApiClient;

  public async Task<IActionResult> OnGetAsync()
  {
    var providers = await _providerApiClient.GetUserProvidersAsync();

    if (!providers.Any())
    {
      return RedirectToPage("/UnlinkedAccountHolding");
    }

    return Page();
  }
}