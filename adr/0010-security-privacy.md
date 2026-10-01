# ADR-0010: Security and privacy by design

* Status: accepted

## Context and Problem Statement

The service will manage provider information and entitlement funding data.

The service must ensure that users are only able to access information and perform actions that they are authorised to access or perform.

Security and privacy controls should be applied consistently throughout the solution architecture.

## Considered Options

* Apply security controls within each business capability
* Rely on client applications to enforce access controls
* Apply security controls retrospectively

## Decision Outcome

Security, privacy, auditability and least-privilege access are mandatory architectural requirements.

Authorisation decisions shall be enforced by trusted service components and shall not rely upon client-side controls.

Access to data and business capabilities shall be subject to appropriate authentication, authorisation and auditing controls.

The architecture should minimise the risk of information being disclosed to unauthorised users.