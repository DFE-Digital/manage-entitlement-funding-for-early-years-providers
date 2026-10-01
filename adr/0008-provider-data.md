# ADR-0008: Consume provider data through an abstraction layer

* Status: accepted

## Context and Problem Statement

The service requires provider information for onboarding, entitlement administration and service operation.

The Alpha phase identified existing and emerging work across DfE to establish authoritative provider data capabilities, including exploration of organisation and provider data services that could potentially become the strategic source of provider reference data.

At the time of writing, no single strategic provider data service has been confirmed as available within the timescales required by this service.

The service must therefore be able to operate using an initial provider data implementation while remaining able to adopt a future departmental provider data capability without significant impact to business functionality.

## Considered Options

* Implement provider data consumption directly against an initial provider data source
* Maintain a service-specific provider register
* Consume provider data through a service-owned abstraction layer
* Delay delivery until a strategic provider data service exists

## Decision Outcome

The service will access provider data through a service-owned abstraction layer. Call this abstraction the "Provider Directory" or "Provider Registry Gateway".

The abstraction layer will define the provider data contract required by the service and shield business functionality from implementation details of upstream provider data sources.

An initial implementation may use the minimum provider data capability required to support service delivery.

Future implementations may consume a strategic DfE provider data service without requiring significant changes to business functionality.

The service will not become the system of record for provider reference information.

Its primary responsibility is entitlement funding administration rather than provider data management.