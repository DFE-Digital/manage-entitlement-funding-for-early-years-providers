using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Reqnroll;
using static Microsoft.Playwright.Assertions;

namespace Dfe.ManageEntitlementFunding.Tests.E2e.Steps;

[Binding]
internal sealed class LoginSteps(IPage page, TestSettings settings)
{
    [When("I enter the developer log in details")]
    public async Task WhenIEnterTheDeveloperLogInDetails()
    {
        await page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name =  new Regex("Subject ID").ToString() }).FillAsync("user-123");
        await page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "User Full Name" }).FillAsync("Jane Doe");
        await page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Email Address" }).FillAsync("Jane.Doe@example.gov.uk");
    }

    [When("I log in")]
    public async Task WhenILogIn()
    {
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Start now"}).ClickAsync();
        await WhenIEnterTheDeveloperLogInDetails();
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Sign in with test identity"}).ClickAsync();
    }

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

    [Then("I am signed in as the developer user")]
    public async Task ThenIAmSignedInAsTheDeveloperUser()
    {
         await Expect(page.GetByText("DEV SESSION:")).ToBeVisibleAsync();
         await Expect(page.GetByText("Jane Doe")).ToBeVisibleAsync();
         await Expect(page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Dev Sign Out" })).ToBeVisibleAsync();
    }
}
