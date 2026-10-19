// ManageEntitlementFunding.Web/Pages/UnlinkedAccountHolding.cshtml.cs
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManageEntitlementFunding.Web.Pages;

[Authorize] // Guarantees the user is logged in via DfE Sign-In before reaching this page
public class UnlinkedAccountHoldingModel : PageModel
{
    public string? UserName { get; private set; }
    public string? UserEmail { get; private set; }
    public string? SubjectId { get; private set; }

    public void OnGet()
    {
        // Extract DfE Sign-In user details from authentication claims
        UserName = User.FindFirst("name")?.Value 
                ?? User.FindFirst(ClaimTypes.Name)?.Value;

        UserEmail = User.FindFirst("email")?.Value 
                 ?? User.FindFirst(ClaimTypes.Email)?.Value;

        SubjectId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                 ?? User.FindFirst("sub")?.Value;
    }
}