# ADR-0012: Expose business capabilities through APIs

* Status: accepted

## Context and Problem Statement

The service is expected to integrate with other systems and evolve independently over time.

The service must also enforce the security and authorisation requirements defined in [ADR-0010](0010-security-privacy.md).

Direct access to business data from consuming components makes it harder to apply security policies consistently and increases the risk of accidental information disclosure.

## Considered Options

* API-first integration model
* Shared database integration
* File-based integration

## Decision Outcome

Business capabilities shall be exposed through defined APIs.

Authorisation, validation, auditing and business rules should be enforced through these APIs.

Direct access to persistence stores from external consumers shall be avoided.

The API layer should provide consistent enforcement points for authentication, authorisation, auditing and business rules,
while supporting loose coupling between components.

In future, the service solution may contain batch processes, message handlers, event consumers, background jobs, and internal domain services, which may use message-driven interfaces. The intention of this ADR is to preserve controlled service boundaries where security policies can be enforced.
It is explicitly *not* specifying that interactions between components will be implemented using HTTP endpoints.