---
title: "ADR-0020: Protect development and test sites with HTTP Basic Auth"
---

## Status

Accepted

## Context and problem statement

The team needs deployed sites for feature development and disposable PR testing
before the full infrastructure design is complete. These sites must not be
accessible to the public.

## Decision drivers

* **Restricted access:** Require an access credential before reaching a test site.
* **Low operational overhead:** Use permissions and deployment tooling already
  available to the team.
* **Low cost:** Avoid additional proxy services or hosting solely for access control.
* **Reversibility:** Keep the temporary gate separate from business authentication.
* **Consistency:** Reuse the approach established by Accessing Childcare.
* **Environment safety:** Keep development credentials and simulated identities
  out of staging and production.

## Considered options

* **Microsoft Entra through App Service Authentication:** Preferable for individual
  identities and centrally managed access, but requires coordination with DfE admins.
* **IP restrictions for site visitors:** No application authentication code, but
  depends on stable, approved egress addresses for developers and reviewers.
* **A Basic Auth reverse proxy:** Avoids application changes but introduces another
  component, deployment configuration and origin-access controls.
* **A small application-level Basic Auth gate:** Reuses an established pattern
  without adding infrastructure or a new identity-provider dependency.

## Decision outcome

Use HTTP Basic Auth over HTTPS as an interim outer access gate for development
and disposable test sites, following the
[Accessing Childcare approach](https://github.com/DFE-Digital/accessing-childcare-entitlement-checker/blob/main/docs/content/reference/decisions/dev-deployment-use-http-basic-auth.md).

This is not the production authentication solution or evidence of compliance
with organisational security standards. Review the decision before introducing
real data, or when the final infrastructure design is agreed.

## Consequences

* **Positive:** The team can protect temporary sites without waiting for new
  directory registrations or adding a proxy service.
* **Positive:** The gate is small, replaceable and independent of provider identity.
* **Negative:** A shared credential does not provide MFA, per-user revocation or
  reliable individual attribution.
* **Negative:** Access changes require credential rotation and redeployment of
  affected sites; browsers may retain credentials.
* **Constraint:** Restrict use to synthetic-data development/test sites over HTTPS.
* **Constraint:** Keep credentials out of source control and logs, and restrict
  access to state and plan files that contain them.

Operational procedures belong in the
[deployment how-to](../../how-to/deploy-development-sites.md); configuration and
limits are in the [deployment reference](../development-deployments.md).
