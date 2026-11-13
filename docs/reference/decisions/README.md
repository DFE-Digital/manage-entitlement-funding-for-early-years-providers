---
title: Decisions
permalink: /reference/decisions/
eleventyNavigation:
  key: Decisions
  parent: Reference
---

# Architecture Decision Records

This repository uses [MADR](https://adr.github.io/madr/) to capture significant architectural decisions.
There is an [ADR template](0000-template.md) that can be copied.

## Records

* [ADR-0001: Use MADR framework for ADRs](0001-record-architecture-decisions.md)
* [ADR-0002: Use .NET on Azure](0002-platform.md)
* [ADR-0003: Provision infrastructure using Infrastructure as Code](0003-iac.md)
* [ADR-0004: Use automated testing as the primary quality gate](0004-automated-testing.md)
* [ADR-0005: Deploy through automated pipelines](0005-automated-deployment.md)
* [ADR-0006: Design for observability](0006-observability.md)
* [ADR-0007: Prefer automation over manual operational processes](0007-automate-where-possible.md)
* [ADR-0008: Consume provider data through an abstraction layer](0008-provider-data.md)
* [ADR-0009: Reuse existing identity and onboarding capabilities](0009-identity-onboarding.md)
* [ADR-0010: Security and privacy by design](0010-security-privacy.md)
* [ADR-0011: Provide attributable audit trails for business actions](0011-audit.md)
* [ADR-0012: Expose business capabilities through APIs](0012-capabilities-through-api.md)
* [ADR-0013: Adopt architectural principles from Alpha](0013-alpha-principles.md)
* [ADR-0014: Enforce provider data isolation at the service boundary](0014-tenant-isolation.md)
* [ADR-0015: Containerisation standards](0015-containerisation-base-images.md)
* [ADR-0016: Use CQRS to separate reads and writes](0016-cqrs.md)
* [ADR-0017: Publish technical documentation](0017-publish-technical-documentation.md)
* [ADR-0018: Bootstrap Terraform state storage using Bicep](0018-bootstrap-terraform-with-bicep.md)
* [ADR-0019: Use ephemeral environments for full-stack testing](0019-ephemeral-environments.md)
* [ADR-0021: Allowing easier editing of content](0021-content-editing.md)

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
