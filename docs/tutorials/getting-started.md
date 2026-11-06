---
title: Getting started guide
layout: sub-navigation
sectionKey: Tutorials
order: 1
includeInBreadcrumbs: true
eleventyNavigation:
  parent: Tutorials
  key: Getting started guide
---

## Development set-up

Clone the repository. The solution file is in the repository root folder.

In the root folder open the IDE. (VS Code will work. Extensions are recommended for working
with this repository: C#, C# Dev Kit, GitHub Actions, HashiCorp Terraform, Red Hat XML, Red
Hat YAML, David Anson's markdownlint.)

## Prerequisites

- .NET 10 SDK - for building and running the apps
- [just](https://just.systems/) - to orchestrate common tasks
- Node.js 24 LTS (which includes npm) - to build the documentation site

<details>
<summary>Windows (PowerShell)</summary>

Install using [WinGet](https://learn.microsoft.com/windows/package-manager/winget/):

```powershell
winget install --id Microsoft.DotNet.SDK.10 --exact --version 10.0.403
winget install --id Casey.Just --exact --version 1.59.0
winget install --id OpenJS.NodeJS.LTS --exact --version 24.22.1
```

Alternatively, use the [.NET SDK installer](https://dotnet.microsoft.com/download/dotnet/10.0),
[just installation instructions](https://just.systems/man/en/packages.html), and
[Node.js installer](https://nodejs.org/en/download).

Restart your terminal and VS Code after installation so they pick up PATH changes.

</details>

<details>
<summary>Linux</summary>

Linux installation steps depend on your distribution:

- Install the **.NET 10 SDK**, not just the runtime, using the
  [.NET Linux installation guide](https://learn.microsoft.com/dotnet/core/install/linux).
- Install **just** using the package-manager instructions for your distribution
  in the [just installation guide](https://just.systems/man/en/packages.html).
- Install **Node.js 24 LTS** using the Linux instructions on the
  [Node.js download page](https://nodejs.org/en/download). Include npm if your
  distribution packages it separately.

Open a new terminal after installation.

</details>

<details>
<summary>macOS</summary>

- Install **.NET SDK 10.0.403** using the macOS package for
  [Apple Silicon](https://builds.dotnet.microsoft.com/dotnet/Sdk/10.0.403/dotnet-sdk-10.0.403-osx-arm64.pkg)
  or [Intel](https://builds.dotnet.microsoft.com/dotnet/Sdk/10.0.403/dotnet-sdk-10.0.403-osx-x64.pkg).
- Download **just 1.59.0** for
  [Apple Silicon](https://github.com/casey/just/releases/download/1.59.0/just-1.59.0-aarch64-apple-darwin.tar.gz)
  or [Intel](https://github.com/casey/just/releases/download/1.59.0/just-1.59.0-x86_64-apple-darwin.tar.gz).
  Extract the archive and place the `just` executable in a directory on your
  PATH, following the [pre-built binary instructions](https://just.systems/man/en/pre-built-binaries.html).
- Install **Node.js 24.22.1**, including npm, using the
  [macOS installer](https://nodejs.org/dist/v24.22.1/node-v24.22.1.pkg).

Open a new terminal after installation.

</details>

Check that all tools are available in the terminal you will use for development:

```shell
just --version
just doctor
```

## Command-line tasks

Run these tasks from the repository root.

### Preparing to build

Running the apps in a dev environment requires setting up a key to enable local
development authentication without relying on DfE Sign-In. This authentication
mechanism is disabled outside the development environment.

To do this, run the below command; which works in PowerShell, Bash, and Zsh and
uses `node` to generate a key without it entering shell history.

> [!WARNING]
> User secrets are local development storage, not encrypted storage; if passing a
> custom key do not use a production key or commit it to source control.

```shell
just init "$(node -p "require('node:crypto').randomBytes(32).toString('base64')")"
```

This command also trusts the development HTTPS certificate, which may require
confirmation or platform-specific setup; see the [.NET HTTPS documentation](https://learn.microsoft.com/aspnet/core/security/enforcing-ssl#trust-the-aspnet-core-https-development-certificate).

### Building and testing

After initialization:

```shell
just build
just test
```

`just build` builds the .NET solution, installs the documentation's locked npm
dependencies, and builds the documentation into `artifacts/docs/`.

## Running the apps

Start the API and web app in separate terminals:

```shell
just run Api
```

```shell
just run
```

`just run` defaults to the web app; `just run Web` is equivalent. Both use the
HTTPS launch profiles. Open <https://localhost:7191> once both apps are running.

### Troubleshooting

When troubleshooting, try:

```shell
just --version
just doctor
```

to check all dependencies have been installed.

## Manual setup

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

(...but see [ADR-0015](../reference/decisions/0015-containerisation-base-images.md) about an implication for containerisation security.)

Also, in developmnent only, it is possible to set up a symmetric signing key for dummy tokens to avoid having to rely
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
