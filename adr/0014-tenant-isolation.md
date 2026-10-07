# ADR-0014: Provider tenant isolation and Authorization Strategy

* Status: accepted

## Context and Problem Statement

The service is a multi-tenant application accessed by early years providers and administrative
staff across England.

A provider user authenticates via DfE Sign-In (DSI) and receives an OpenID Connect (OIDC)
identity token containing user claims (such as `sub` / `NameIdentifier`). But a valid login
alone does not guarantee that the authenticated user has rights to access or modify data for
a specific provider (identified by `ProviderId` or `Urn`).

We need to establish a robust tenant isolation strategy across the Web frontend and API backend
to ensure:

* Users can only read and mutate data belonging to providers they are explicitly authorised to manage.
* Cross-tenant data leakage is prevented at both the application edge (web client) and the service boundary (API).
* Local development and authomated testing can exercise authorization rules cleanly without relying on external infrastructure.

## Decision Drivers

## Considered Options

* Option 1: rely on frontend route filtering only. Filter data on the web application
based on the user's claims, leaving the API endpoints open to any authenticated bearer token.
* Option 2: Claim-embedded provider access list. Encode the user's permitted `ProviderId` list
directly into the JWT claims issued by the Identity Provider during login.
* Option 3: Two-tier policy authorization via API & custom requirements. Validate user-to-provider
mappings in a centralised backend service using ASP.NET Core authorisation policies
(`IAuthorizationHandler`), enforcing tenant checks on every API request.

## Decision Outcome

Chosen option: option 3. Two-tier policy authorisation via API and custom requirements.

This approach provides true defence in depth: the Web UI directs users based on their
associated providers, but the API independently enforces tenant boundaries on every request
by evaluating a custom authorisation policy.

### Key operational rules

* Explicit API enforcement
  * Every API route containing a provider resource context must enforce authorisation using
  `[Authorize(Policy = "CanAccessProvider")]` or explicit claim/relationship verification.
  * The API extracts the user identifier (`sub`) from the validated JWT Bearer token and
  checks database/directory mappings before executing any business logic or returning provider records.
* Web application navigation and routing
  * When an authenticated user logs into the Web frontend, the application calls `GET /api/providers/my-providers`.
  * If the API returns an empty array `[]`, the web app redirects the user to `/UnlinkedAccountHolding` in
  compliance with DfE user journey standards.
  * If the user attempts to access a provider URL `api/providers/{providerId}` for which they
  lack entitlement, the API returns `403 Forbidden` and the web app displays the standard
  GDS `403` Access Denied page.
* Domain record standardisation
  * Shared contracts (such as `ProviderSummaryDto`) reside in `ManageEntitlementFunding.Domain.DataTransfer`
  to ensure a single, consistent schema for provider boundaries across web, API, and test projects.

### Aims

* Complete prevention of Insecure Direct Object Reference (IDOR) vulnerabilities between provider tenants.
* API endpoints securely isolated and remain protected even if invoked directly outside the Web frontend.
* Simplified testability: `ProviderAssociationHandler` can be unit tested against mock `ClaimsPrincipal`
objects without spinning up OIDC workflows.

This is considered worth the extra initial API call (`GET /api/providers/my-providers`) on session launch.
