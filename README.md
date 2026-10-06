# Manage entitlement funding for early-years providers

A service that allows authenticated early years providers to manage the data
they need to submit claims to local authorities for their provision of statutory
childcare entitlements.

## Project structure

```text
manage-entitlement-funding-for-early-years-providers/
├─ adr/ -- Architecture decision records
├─ src/
├─ terraform/ - Terraform project used to implement the Azure infrastructure as code.
├─ tests/
```

## Development set-up

Clone the repository. The solution file is in the `src` folder. (This may change -- when
we start adding unit tests in the `/tests` folder, it may make more sense for it to be
in the root folder.)

In the `src` folder open the IDE. (VS Code will work. Extensions are recommended for working
with this repository: C#, C# Dev Kit, GitHub Actions, HashiCorp Terraform, Red Hat XML, Red
Hat YAML, David Anson's markdownlint.)

For the two projects `ManageEntitlementFunding.Web.csproj` and
`ManageEntitlementFunding.Api.csproj`, manage user secrets and enter the following:

```json
{
    "DfESignIn": {
        "Authority": "https://Auth",
        "ClientId": "ClientId",
        "ClientSecret": "ClientSecret"
    }
}
```

Obviously, you will replace the values with the correct URI, client ID and client
secrets for DfE Sign-In.

The solution has been set up to enable running of the API and Web projects in debug
simultaneously. You will need to select the **Run and Debug** panel in VS Code, and
check that the drop-down list has selected *Launch Both (API + Web)*; then hitting
F5 should do exactly that. You may need to explicitly *build* the solution first.
You may also find you need to explicitly trust the HTTPS developer certificate. To
do that, from the command prompt type:

```cmd
dotnet dev-certs https --trust
```

(...but see [ADR-0015](./adr/0015-containerisation-base-images.md) about an implication for containerisation security.)
