---
title: Deploy development sites
layout: sub-navigation
sectionKey: How-to guides
includeInBreadcrumbs: true
eleventyNavigation:
  parent: How-to guides
  key: Deploy development sites
---

Use this guide to configure the persistent development site and disposable
internal PR sites. Use synthetic data only. For the configuration model and
security limits, see the [development deployment reference](../reference/development-deployments.md).

## Azure Prerequisites

- A subscription to deploy into
- The following resource providers registered in the subscription:
  - Microsoft.Web
- An app registration configured to allow OIDC access from GitHub

## Configure deployment access

1. In the repository's **Settings > Environments > development**, configure:

   | Secret | Purpose |
   | --- | --- |
   | `AZURE_CLIENT_ID` | Existing Azure deployment identity's client ID |
   | `AZURE_TENANT_ID` | Tenant used for Azure deployment OIDC |
   | `AZURE_SUBSCRIPTION_ID` | Development subscription |
   | `DEVELOPMENT_BASIC_AUTH_PASSWORD` | Generated shared password of at least 32 characters |

2. Generate and distribute the site password using approved password-management
   tooling.

## Deploy and verify a site

1. Review the infrastructure changes before triggering deployment.
2. Open or update a trusted, same-repository, non-draft PR to deploy its site.
   Updates to `main` deploy the persistent site.
3. Once deployed, the workflow outputs the web url in the job summary.
4. Access the web site and use HTTP basic auth for the initial gate.
5. Afterwards, log in via application-level security (at time of writing, a mock DfE sign-in)

Closing a PR removes an ephmeral PR instance. The site deployed from `main` is never
torn down.

## Rotate the shared site password

1. Generate a new password of at least 32 characters.
2. Update `DEVELOPMENT_BASIC_AUTH_PASSWORD` in the development GitHub environment.
3. Redeploy `main` and every active PR site that needs the new password. Updating
   the GitHub secret alone does not change running apps.
4. Distribute the replacement using approved password-management tooling.

## Check the gate locally

Set `DevelopmentBasicAuthPassword` in the Web project's user secrets and enable
`DevelopmentBasicAuthEnabled` for the local process. Never put credentials in
checked-in application settings. Unset the flag afterwards to restore the normal
local flow.
