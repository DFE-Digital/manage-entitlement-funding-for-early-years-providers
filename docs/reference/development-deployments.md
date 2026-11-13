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

The development environment; including deployments triggered by pushes to `main`
and deployments from PR branches, are intended to support feature work.

They do not constitute the final infrastructure design. You should only use synthetic
data, and connect only to staging or development services.

For setup and acceptance checks, see
[Deploy and verify development sites](../how-to/deploy-development-sites.md).
The access-control decision is recorded in
[ADR-0020](decisions/0020-development-site-basic-auth.md).

The persistent `main` deployment and eligible internal PR deployments use the same
development subscription and GitHub environment. Each has a dedicated application
resource group and Terraform state key. The shared backend is outside the PR
lifecycle; closing a PR removes its application resources, not the backend.

## Hosting and cost

Each deployment uses the same development subscription and `development` GitHub environment.
They create a dedicated Azure resource group and terraform state key. Note that the
shared backend for deployments (bicep bootstrap) is outside of the PR lifecycle.

The Web and API apps share one Linux App Service plan per deployment. The default
SKU is `F1`; the intention is to minimise costs. Free hosting has no SLA, no
Always On, and a daily CPU quota per app.

The Web app is served directly from its HTTPS `azurewebsites.net` hostname.
Front Door is deliberately omitted: it adds a standing charge per deployment
without being needed for this temporary access gate.

The Terraform backend's storage, private endpoint and diagnostics still incur
charges.

We can and should reconsider this if it proves inconvenient; but at the same time
bear in mind that ephemeral deployments are not free.

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

Share the password only through an approved
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

## Credentials and state

The browser password is supplied through
the development GitHub environment; see the [deployment how-to](../how-to/deploy-development-sites.md).

The workflow passes the shared site password as the sensitive Terraform variable
`development_basic_auth_password`, validates it before Azure login, and never
outputs it.

There is also a shared key used for communication between the web app and the api.
This signing key is generated separately for each development
deployment; and is unrelated to the browser's Basic Auth password.

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

## Pipeline trust and teardown

Only trusted same-repository, non-draft PRs deploy automatically. Fork PRs do not.
The existing Terraform cleanup flow is retained. Internal PR infrastructure and
scripts execute with the development deployment identity, including on teardown.
This deliberately trusts internal contributors with those permissions.
