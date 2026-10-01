# ADR-0005: Deploy through automated pipelines

* Status: accepted

## Context and Problem Statement

The service requires repeatable and auditable deployment processes.

Manual deployments introduce risk and reduce operational consistency.

## Considered Options

* Automated deployment pipelines
* Semi-automated releases
* Manual deployments

## Decision Outcome

All environments will be deployed through automated pipelines.

Source control and deployment pipelines are the authoritative definition of the running service.