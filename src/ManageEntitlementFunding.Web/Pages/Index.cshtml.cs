using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageEntitlementFunding.Web.Pages;

[AllowAnonymous] // Public landing or "start" page
public class IndexModel : PageModel
{
    public void OnGet()
    {

    }
}
