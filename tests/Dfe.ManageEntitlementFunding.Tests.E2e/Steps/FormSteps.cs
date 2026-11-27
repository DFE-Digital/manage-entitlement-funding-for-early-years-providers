using Microsoft.Playwright;
using Reqnroll;
using static Microsoft.Playwright.Assertions;

namespace Dfe.ManageEntitlementFunding.Tests.E2e.Steps;

[Binding]
internal sealed class FormSteps(IPage page, TestSettings settings)
{
    [Given("I am on the provider portal homepage")]
    public async Task GivenIAmOnTheProviderPortalHomepage()
    {
        await page.GotoAsync("/");
    }

    [When("I click the {string} button")]
    public async Task WhenIClickTheStringButton(string buttonText)
    {
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = buttonText}).ClickAsync();
    }

    [When("I click the {string} link")]
    public async Task WhenIClickTheStringLink(string linkText)
    {
        await page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = linkText}).ClickAsync();
    }

    [Then("I can see the {string} button")]
    public async Task ThenICanSeeTheStringButton(string buttonText)
    {
        await Expect(
            page.GetByRole(AriaRole.Button,new PageGetByRoleOptions { Name = buttonText })
        ).ToBeVisibleAsync();
    }

    [Then("the page title is {string}")]
    public async Task ThenThePageTitleIsString(string text)
    {
        await Expect(page).ToHaveTitleAsync(text);
    }

    [Then("I see the homepage")]
    public async Task ThenISeeTheHomepage()
    {
        await Expect(page).ToHaveURLAsync("/");
    }
}
