# ADR-0014: Enforce provider data isolation at the service boundary

* Status: accepted

## Context and Problem Statement

The service is a multi-tenant application used by people associated with
early years providers.

Authentication establishes the identity of a user, but does not establish
that the user is authorised to access data belonging to a particular provider.

The service must prevent users from reading or modifying data belonging to
providers with which they do not have an appropriate relationship.

Client-side navigation and filtering cannot provide this security boundary
because API endpoints may be invoked independently of the Web application.

## Decision

Access to provider-scoped data and operations shall be authorised by the API
using the authenticated user's identity and the relationships relevant to the
requested operation.

The Web application may use provider relationships to control navigation and
user experience, but these controls shall not be relied upon for security.

Each application operation shall enforce the data-authorisation rules
appropriate to that operation before disclosing or modifying protected data.

Where practical, authorisation constraints should form part of data retrieval,
rather than retrieving data outside the caller's authorised scope and filtering
it afterwards.

The implementation of these rules may vary between application operations.
This ADR does not mandate a single global provider-access policy or a particular
persistence mechanism.

## Consequences

* Authentication alone does not confer access to provider data.
* Provider-scoped API operations must enforce their own applicable data
  authorisation requirements.
* Direct invocation of the API cannot bypass controls implemented by the Web
  application.
* Query and command implementations must be designed so that unauthorised
  provider data cannot inadvertently be disclosed.
* Persistence implementations must support efficient enforcement of these
  data-access boundaries.
* Automated tests should verify attempts to access resources belonging to
  providers to which a user is not authorised.
