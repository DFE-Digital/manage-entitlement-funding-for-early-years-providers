# ADR-0013: Adopt architectural principles from Alpha

* Status: accepted

## Context and Problem Statement

The service is transitioning from Alpha into a potential Beta phase.

A significant amount of architectural learning has been obtained during Alpha and should continue to influence future decisions.

Without explicit documentation there is a risk that future architectural choices diverge from the intent established during Alpha.

## Decision Outcome

Architectural decisions should be guided by the following principles:

* Reuse existing capabilities before building new capabilities.
* Prefer authoritative data sources over duplicated data.
* Prefer shared departmental platforms over service-specific platforms.
* Automate repeatable operational activities.
* Optimise for maintainability and operability.
* Make dependencies and risks explicit.
* Prefer loosely coupled integrations.

Future ADRs should explain how they align with or intentionally depart from these principles.