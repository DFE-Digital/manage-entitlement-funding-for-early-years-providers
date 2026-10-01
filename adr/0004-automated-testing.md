# ADR-0004: Use automated testing as the primary quality gate

* Status: accepted

## Context and Problem Statement

The service is expected to evolve continuously and support frequent deployment.

Reliance upon manual testing creates delivery bottlenecks and increases the risk of regression defects.

## Considered Options

* Automated testing with pipeline quality gates
* Mixed automated and manual release validation
* Predominantly manual testing

## Decision Outcome

Automated testing will be the primary release quality mechanism.

Deployment pipelines shall enforce automated quality gates before release to subsequent environments.