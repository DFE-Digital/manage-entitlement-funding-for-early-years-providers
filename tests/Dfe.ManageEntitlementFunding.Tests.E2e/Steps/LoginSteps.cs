using Microsoft.Playwright;
using Reqnroll;
using static Microsoft.Playwright.Assertions;

namespace Dfe.ManageEntitlementFunding.Tests.E2e.Steps;

[Binding]
internal sealed class LoginSteps(IPage page, TestSettings settings)
{
    [Then("I can see the log in fields")]
    public async Task ThenICanSeeTheLogInFields()
    {
        List<string> fields = [
            "Subject ID (sub / NameIdentifier)",
            "User Full Name",
            "Email Address"
        ];

        foreach (var field in fields)
        {
            await Expect(
                page.GetByRole(AriaRole.Textbox,new PageGetByRoleOptions { Name = field })
            ).ToBeVisibleAsync();
        }
    }
}
