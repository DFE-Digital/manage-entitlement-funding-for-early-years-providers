# Architecture Decision Records

This repository uses [MADR](https://adr.github.io/madr/) to capture significant architectural decisions.
There is an [ADR template](0000-template.md) that can be copied.

## When to create an ADR

Create an ADR when a decision:

* Is expensive to reverse.
* Has a significant impact on architecture.
* Introduces a long-term dependency.
* Establishes a design principle or constraint.
* Involves choosing between multiple credible options.
* Is likely to be questioned by future contributors.
* Requires future teams to understand why a particular approach was chosen.

Examples include:

* Hosting platform choices.
* Security architecture decisions.
* Data ownership boundaries.
* Integration approaches.
* Identity and access management decisions.
* Audit and compliance capabilities.
* Significant technology selections.

## What should not normally be ADRs

Avoid recording:

* Routine implementation details.
* Coding standards and style conventions.
* Sprint-specific delivery decisions.
* Temporary workarounds.
* Operational procedures.
* Decisions that can be easily derived from the source code.
* Sensitive security implementation details.

## Public repository considerations

ADRs should describe architectural intent and rationale.

Avoid including:

* Secrets or credentials.
* Known vulnerabilities.
* Security exceptions.
* Fraud detection approaches.
* Internal operational processes.
* Information that would materially assist an attacker.

Focus on objectives, constraints and architectural outcomes rather than implementation details.