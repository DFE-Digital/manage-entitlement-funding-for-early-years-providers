---
title: "ADR-0019: Use ephemeral environments for full-stack testing"
---

## Status

Accepted

## Context and problem statement

We need to test changes against a deployed application and its supporting
infrastructure before they are merged. The stack is expected to include at least
a Microsoft SQL Server (MSSQL) database as the service develops.

Unit tests and tests using substitutes cannot verify every interaction between
the application, database, deployment configuration and Azure infrastructure.
A shared development deployment also allows concurrent changes and test data to
interfere with each other, making failures harder to attribute to a particular PR.

Ephemeral environments allow each PR to be tested against its own full-stack
deployment, including its database schema, migrations and test data. This supports
[ADR-0004: Use automated testing as the primary quality gate](0004-automated-testing.md)
and [ADR-0005: Deploy through automated pipelines](0005-automated-deployment.md).

## Decision drivers

* **Confidence:** Exercise the deployed application and real supporting services
  together, rather than relying solely on mocks or local execution.
* **Isolation:** Prevent concurrent PRs from overwriting each other's application
  deployments, database schemas or test data.
* **Repeatability:** Provision the stack and prepare test data automatically.
* **Feedback:** Find integration and deployment failures before merging changes.
* **Cost control:** Remove temporary resources when they are no longer needed.

## Considered options

* **Local or CI-only tests:** Fast and useful, but do not fully exercise the
  deployed Azure stack and its configuration.
* **One shared development deployment:** Uses fewer resources, but concurrent
  changes and database updates can interfere with each other's tests.
* **Ephemeral environment per PR:** Provides a dedicated full-stack test target,
  at the cost of additional provisioning time, resources and lifecycle management.

## Decision outcome

Provision a temporary deployment for each eligible internal PR. Update that
deployment as the PR changes and destroy its application resources when the PR
is closed or merged.

Each PR deployment has its own application resource group and Terraform state.
As the stack develops, it will include the application and supporting services
needed for full-stack tests, including a dedicated MSSQL database with independently
managed schema and test data. Tests must use synthetic or otherwise approved
non-production data.

Automated full-stack tests will run against the deployed application after
provisioning and application deployment complete. These tests complement, rather
than replace, faster unit and integration tests earlier in the pipeline.

Initially, PR deployments and the persistent `main` deployment use the same
`development` Azure subscription and GitHub environment. An ephemeral environment
is a temporary application deployment, not a separate GitHub environment or Azure
subscription. The shared Terraform backend remains outside the PR lifecycle,
as described in [ADR-0018](0018-bootstrap-terraform-with-bicep.md).

Only trusted, same-repository PRs deploy automatically. Fork contributions must
first be reviewed and brought onto an internal branch under the
[contribution policy](../../tutorials/contributing.md).

This decision establishes the intended testing architecture. The current draft
provisions resource groups and separate state; application hosting, the database
and deployed full-stack tests will be added as the service develops. The database
hosting model is a separate decision.

## Consequences

* **Positive:** PRs can be tested against their own deployed stack before merge.
* **Positive:** Application changes, database migrations and infrastructure changes
  can be exercised together without changing another PR's deployment.
* **Positive:** Temporary deployments also provide a target for manual review.
* **Negative:** Parallel deployments increase Azure resource usage and cost.
* **Negative:** Provisioning, database setup and full-stack tests add pipeline time.
* **Negative:** Reliable cleanup and recovery from failed operations require
  ongoing maintenance.
* **Constraint:** Resource and data separation within development is not equivalent
  to a separate security boundary for each PR.
