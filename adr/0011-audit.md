# ADR-0011: Provide attributable audit trails for business actions

* Status: accepted

## Context and Problem Statement

The service will enable users to view and manage information associated with entitlement funding.

The service must provide confidence that material business actions can be traced to the actor responsible for those actions.

Operational support, assurance activities and future investigations may require evidence of what actions were performed, when they were performed and by whom.

## Considered Options

* Audit only technical operations
* Audit material business actions
* No dedicated audit capability

## Decision Outcome

The service shall record auditable business events for material actions.

Audit records should allow the service to determine:

* who performed the action
* what action was performed
* when the action occurred
* which business entity was affected
* sufficient contextual information to understand the change

Audit records shall be protected from unauthorised access, and unauthorised modification, and shall be retained in accordance
with applicable retention requirements.

Audit information is a business capability, separate from operational logging concerns.