# Running and building and debugging

## How to build playwright tests

Navigate to the E2E project, located within the _tests_ folder

```
cd tests/Dfe.ManageEntitlementFunding.Tests.E2e
```

and build with

```
dotnet build
```
## How to run playwright tests

Navigate to the E2E project, located within the _tests_ folder

```
cd tests/Dfe.ManageEntitlementFunding.Tests.E2e
```

and run with

```
dotnet test
```

## How to debug through a browser UI

```
PWDEBUG=1 dotnet test
```

## How to specify particular tests

If you have a test scenario called **"Scenario: Developer log in"** then you can run or debug just this one test by adding the flag
`--filter "DisplayName~Developer log in"`

Full command
```
dotnet test --filter "DisplayName~Developer log in"
```
Full debug command
```
PWDEBUG=1 dotnet test --filter "DisplayName~Developer log in"
```

# Writing tests

## Locate your element 

### By ID

It is best practice to locate your elements by ID. You can add an ID to any element by adding the following attribute to the HTML

```
data-testid="start"
```

where "start" here is the ID you will target in your Step C# like this:

```
await page.GetByTestId("start");
```

### By Role

You likely have multiple of one role on a page, so GetByRole is usually paired with a Name, specified via Options:

```
await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Start now"});
```

### Text Input Fields

If is a text field, it is recommended to use ByLabel like so:

```
await page.GetByLabel("Email Address");
```

### Alternate methods

Other methods include by Text or by Role: [Playwright Docs - Locators](https://playwright.dev/dotnet/docs/locators)

## Actions on your located element

Common actions include to click, or fill, or to check is visible. Find more here: [Playwright Docs - Actions](https://playwright.dev/dotnet/docs/input)

### Performing actions (aka: "When")
```
await page.GetByTestId("sign-in").ClickAsync();
await page.GetByRole(AriaRole.Textbox).FillAsync("Peter");
```

### Confirming actions (aka: "Then")
```
await Expect(page).ToHaveTitleAsync(text);
await Expect(page.GetByText("DEV SESSION:")).ToBeVisibleAsync();
```

