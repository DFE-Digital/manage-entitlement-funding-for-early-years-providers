// using System.Text.RegularExpressions;
// using System.Threading.Tasks;
// using Microsoft.Playwright;
// using Microsoft.Playwright.Xunit;
// using Xunit;
//
// namespace Dfe.ManageEntitlementFunding.Tests.E2e;
//
// public class LogInTests: PageTest
// {
//     [Fact]
//     public async Task HasTitle()
//     {
//         await Page.GotoAsync("https://localhost:7191");
//
//         await Expect(Page).ToHaveTitleAsync(new Regex("Manage entitlement funding for early years providers - GOV.UK"));
//     }
//
//     [Fact]
//     public async Task GoToLoginPage()
//     {
//         await Page.GotoAsync("https://localhost:7191");
//
//         await Page.GetByRole(AriaRole.Button, new() { Name = "Start now" }).ClickAsync();
//
//         await Expect(Page).ToHaveURLAsync(new Regex("https://localhost:7191/DevLogin"));
//     }
//
//     [Fact]
//     public async Task DevLoginPageContents()
//     {
//         await Page.GotoAsync("https://localhost:7191/DevLogin");
//
//         await Expect(Page).ToHaveTitleAsync(new Regex("Development Sign-In"));
//         await Expect(Page.GetByText("for development testing only")).ToBeVisibleAsync();
//         await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Simulated DfE Sign-In" })).ToBeVisibleAsync();
//         await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Sign in with test identity" })).ToBeVisibleAsync();
//     }
//
//     [Fact]
//     public async Task PerformDevLogin()
//     {
//         await Page.GotoAsync("https://localhost:7191/DevLogin");
//
//         await Page.GetByRole(AriaRole.Textbox, new() { Name = "" + new Regex("Subject ID") }).FillAsync("user-123");
//         await Page.GetByRole(AriaRole.Textbox, new() { Name = "User Full Name" }).FillAsync("Jane Doe");
//         await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email Address" }).FillAsync("Jane.Doe@example.gov.uk");
//
//         await Page.GetByRole(AriaRole.Button, new() { Name = "Sign in with test identity" }).ClickAsync();
//
//         await Expect(Page.GetByText("DEV SESSION:")).ToBeVisibleAsync();
//         await Expect(Page.GetByText("Jane Doe")).ToBeVisibleAsync();
//     }
// }
