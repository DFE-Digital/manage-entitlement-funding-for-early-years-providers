using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace Dfe.ManageEntitlementFunding.Tests.E2e;

public class LogInTests: PageTest
{
    [Fact]
    public async Task HasTitle()
    {
        await Page.GotoAsync("https://localhost:7191");

        await Expect(Page).ToHaveTitleAsync(new Regex("Manage entitlement funding for early years providers - GOV.UK"));
    }

    [Fact]
    public async Task GoToLoginPage()
    {
        await Page.GotoAsync("https://localhost:7191");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Start now" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Simulated DfE Sign-In" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Sign in with test identity" })).ToBeVisibleAsync();
    }

    [Fact]
    public async Task PerformLogin()
    {
        await Page.GotoAsync("https://localhost:7191/DevLogin");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Sign in with test identity" }).ClickAsync();

        await Expect(Page.GetByText("DEV SESSION: Signed in as Jane Doe")).ToBeVisibleAsync();
    }
}
