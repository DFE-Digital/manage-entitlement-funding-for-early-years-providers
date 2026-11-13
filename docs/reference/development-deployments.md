---
title: Development deployment reference
layout: sub-navigation
sectionKey: Reference
order: 3
includeInBreadcrumbs: true
eleventyNavigation:
  parent: Reference
  key: Development deployment reference
---

## Purpose and limits

This is an interim deployment for feature work, not the final infrastructure
design or a production security baseline. Use synthetic data only. Do not connect
production services or load real provider, child or funding records.

For setup and acceptance checks, see
[Deploy and verify development sites](../how-to/deploy-development-sites.md).
The access-control decision is recorded in
[ADR-0020](decisions/0020-development-site-basic-auth.md).

The persistent `main` deployment and eligible internal PR deployments use the same
development subscription and GitHub environment. Each has a dedicated application
resource group and Terraform state key. The shared backend is outside the PR
lifecycle; closing a PR removes its application resources, not the backend.

## Hosting and cost

The Web and API apps share one Linux App Service plan per deployment. The default
SKU is `F1`; override `app_service_sku` with `B1` if quotas or cold starts obstruct
development. Free hosting has no SLA, no Always On, and a daily CPU quota per app.
It is not suitable for production or sustained load tests. Check regional and
subscription quotas before creating many PR environments.

The Web app is served directly from its HTTPS `azurewebsites.net` hostname.
Front Door is deliberately omitted: it adds a standing charge per deployment
without being needed for this temporary access gate. Applying this configuration
to a deployment that already has Front Door removes those resources and changes
the published URL. Review the Terraform plan before applying.

The Terraform backend's storage, private endpoint and diagnostics still incur
charges. F1 does not make the entire environment free. Use Azure Cost Management
to monitor the development subscription and arrange an appropriate budget alert.
No new central monitoring platform is introduced by this interim deployment.

## Shared-password access gate

The Web app follows Accessing Childcare's
[development Basic Auth approach](https://github.com/DFE-Digital/accessing-childcare-entitlement-checker/blob/main/src/Dfe.Acec.Web/DevelopmentExtensions.cs).
The browser prompts for a username and password before the existing simulated
application login. Any username is accepted; only the shared password is checked.
This does not authenticate a real provider or replace future DfE Sign-In.

Terraform sets `DevelopmentBasicAuthEnabled=true` and supplies
`DevelopmentBasicAuthPassword` through App Service settings. When enabled, a
missing or blank password fails application startup rather than opening access.
The gate cannot be enabled outside the ASP.NET Core Development environment.
Local development leaves it disabled by default.

Unlike the source implementation, every Web path is gated, including static
assets and the development login. There are no user-agent-based probe exemptions
or anonymous health exceptions. Responses use `Cache-Control: no-store`; rejected
requests are logged without logging credentials. Malformed credentials return
401, not an exception page.

Shared Basic Auth over HTTPS is a temporary access control, not individual
identity management: there is no MFA, per-user revocation or reliable attribution.
Browsers can retain credentials, so signing out of the simulated application does
not sign out of the outer gate. Share the password only through an approved
channel and rotate it when team access changes or disclosure is suspected.

## Environment and signing-key safeguards

Terraform accepts `development`, `staging` and `production`, mapping them to the
ASP.NET Core environments `Development`, `Staging` and `Production`. The existing
deployment and cleanup workflows still target development only. Accepting other
environment identifiers does not establish a staging or production deployment:
their pipelines, real sign-in configuration, hosting and operational controls
remain separate work.

Only development creates the `random_password.development_jwt_signing_key`
resource and injects `DevelopmentJwt__SigningKey` into either app. Staging and
production omit both the resource and the setting. Development Basic Auth
settings are also omitted outside development, and its password is not required
for those environments.

Both apps reject startup if `DevelopmentJwt:SigningKey` has any non-null value
outside Development, including an empty string. Remove the setting entirely;
blanking it is not sufficient. Their existing runtime authentication checks also
prevent symmetric development tokens from being used outside Development.

The Terraform `moved` block preserves an existing development key when its
resource address changes to the counted form `[0]`. This is a state-address
migration, not a request to rotate the key. Removing a key resource or app setting
does not erase historical state versions.

Development retains its existing resource names. Staging and production names
include their environment to avoid colliding with development's global App
Service names.

## Credentials and state

The workflow passes the shared site password as the sensitive Terraform variable
`development_basic_auth_password`, validates it before Azure login, and never
outputs it. The signing key is generated separately for each development
deployment; it is not the browser's Basic Auth password.

Terraform's sensitive marker redacts normal output, but credentials are still
held in Terraform state and saved plan files. Keep backend access restricted and
do not publish plan files as artifacts. The browser password is supplied through
the development GitHub environment; see the [deployment how-to](../how-to/deploy-development-sites.md).

## API and deployment access

The API is not protected by the browser's Basic Auth header. In development it
retains the existing signed development JWT validation. Its App Service ingress rules allow
the Web app's outbound IPv4 addresses and deny other callers. These addresses
can be shared with other Azure apps, so the allowlist supplements authentication;
it is not proof of caller identity or a private-network boundary.

Terraform derives the ingress rules from the Web app. Its API URL uses the
standard public-Azure App Service hostname so the Web app can be created before
the API, avoiding a dependency cycle. If the hosting model or hostname convention
changes, revisit both the URL and these ingress rules.

FTP is disabled and both FTP and SCM/WebDeploy basic publishing credentials are
disabled. The workflows deploy ZIP packages using Azure OIDC authentication.
SCM remains network-reachable for GitHub-hosted runners and requires Azure
authorization; it is separate from application ingress and the browser gate.

Azure deployment still uses Entra-backed OIDC, as before. The browser access gate
does not require a new Entra app registration, user assignments or directory
permissions.

## Pipeline trust and teardown

Only trusted same-repository, non-draft PRs deploy automatically. Fork PRs do not.
The existing Terraform cleanup flow is retained. Internal PR infrastructure and
scripts execute with the development deployment identity, including on teardown.
This deliberately trusts internal contributors with those permissions.

Confirm the deployment identity's actual role assignments are limited to the
intended development resources/subscription. Configuration targeting development
does not enforce that boundary. Different state keys are not an authorization
boundary when the identity can access the shared backend.
