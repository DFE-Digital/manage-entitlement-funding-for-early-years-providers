# Manage entitlement funding for early-years providers

A service that allows authenticated early years providers to manage the data
they need to submit claims to local authorities for their provision of statutory
childcare entitlements.

## Project structure

```text
manage-entitlement-funding-for-early-years-providers/
├─ adr/ -- Architecture decision records
├─ prototypes/ -- Proofs of concept and prototype implementations
├─ src/
├─ terraform/ - Terraform project used to implement the Azure infrastructure as code.
├─ tests/
```

## Development set-up

Clone the repository. The solution file is in the repository root folder.

In the root folder open the IDE. (VS Code will work. Extensions are recommended for working
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

Also, in development only, it is possible to set up a symmetric signing key for dummy tokens to avoid having to rely
on DfE Sign-In. This key should live in your project's secrets.

To generate the key, from a Terminal:

```shell
[Convert]::ToBase64String(
  [System.Security.Cryptography.RandomNumberGenerator]::GetBytes(32)
)
```

The output will be a string of characters. Copy this.

To add this to user secrets in the API project:

```shell
cd src/ManageEntitlementFunding.Api

dotnet user-secrets set "DevelopmentJwt:SigningKey" "your-generated-key"
```

And to add it to user secrets in the Web project:

```shell
cd src/ManageEntitlementFunding.Web

dotnet user-secrets set "DevelopmentJwt:SigningKey" "your-generated-key"
```

This will set the secret ready to be used for local authentication in development, the development
authentication mechanism is explicitly disabled outside the development environment.
