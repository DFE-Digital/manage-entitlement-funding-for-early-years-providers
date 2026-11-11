---
title: "ADR-0018: Bootstrap Terraform state storage using Bicep"
---

## Status

Accepted

## Context and problem statement

Terraform needs a remote backend to store state and coordinate locking between
deployments. Our Azure Storage account and state container must exist before
the application Terraform configuration can initialise against that backend.

This creates a bootstrap dependency: the application configuration cannot rely
on its remote backend while also being responsible for initially creating it.
Manual setup would introduce a separate, less reproducible provisioning process.

[ADR-0003](0003-iac.md) approves Terraform and Bicep as Infrastructure as Code
technologies. We need a repeatable bootstrap process that fits the automated
deployment approach in [ADR-0005](0005-automated-deployment.md).

## Decision drivers

* **Repeatability:** Provision the backend without manual resource creation.
* **Maintainability:** Keep the bootstrap small, declarative and version-controlled.
* **Lifecycle separation:** Retain state infrastructure independently of application
  deployments, including temporary PR deployments.
* **Azure integration:** Use native deployment tooling without requiring another
  Terraform backend.
* **Consistency:** Reuse an established approach from other DfE services.

## Considered options

* **Manual creation:** Simple initially, but difficult to reproduce and audit.
* **Azure CLI scripts:** Automatable, but require imperative resource creation and
  custom handling of existing resources.
* **Separate Terraform bootstrap configuration:** Retains one IaC language, but
  requires its own state management and potentially a state migration.
* **Bicep bootstrap:** Declarative Azure deployment without a Terraform state
  dependency, at the cost of maintaining two IaC languages.

## Decision outcome

Use a dedicated, subscription-scoped Bicep deployment to provision the Terraform
backend before Terraform initialisation.

This follows the approach described in Accessing Childcare Entitlement Checker's
[Bootstrap Terraform state storage in Azure decision](https://github.com/DFE-Digital/accessing-childcare-entitlement-checker/blob/main/docs/content/reference/decisions/bootstrap-tf.md).

Bicep owns the backend resource group, storage account and state container,
associated private networking, diagnostics and the deployment identity's
blob-data role assignment. Application Terraform configurations do not manage
these resources.

The pipeline runs the Bicep deployment on each deployment or cleanup operation,
creating or updating the backend declaratively. Its outputs supply the backend
resource group and storage account names to Terraform.

Application deployments have separate state keys within the shared backend.
Destroying a PR deployment removes its application resources, not the backend.

## Consequences

* **Positive:** Backend provisioning is automated, repeatable and reviewed alongside
  the application infrastructure.
* **Positive:** No local Terraform state or separate Terraform bootstrap backend
  needs to be maintained.
* **Positive:** Backend resources have a lifecycle independent of application
  deployments and PR cleanup.
* **Negative:** Contributors must maintain both Bicep and Terraform configurations.
* **Negative:** Each operation includes an additional Azure deployment step.
* **Constraint:** Keep Bicep focused on backend prerequisites; application resources
  remain Terraform's responsibility.
* **Constraint:** Changes to backend names or location require deliberate state
  migration; creating a new backend does not move existing Terraform state.
