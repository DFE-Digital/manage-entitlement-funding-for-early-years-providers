---
title: "ADR-0021: Use Azure SQL Database for relational persistence"
---

## Status

Accepted

## Context and problem statement

The service needs persistent storage for entitlement funding data. We need to
choose both a data model and a managed database service within Azure, consistent
with [ADR-0002: Use .NET on Azure](0002-platform.md).

Funding data has relationships between providers, entitlements and funding
records. Maintaining consistency across related records and querying those
relationships are more important than storing independently evolving documents.

There is little to distinguish Azure SQL Database (managed Microsoft SQL Server)
and Azure Database for PostgreSQL for our current requirements. Both support
relational modelling, transactions, constraints and integration with .NET.
The choice between them is therefore primarily one of convention, rather than a
claim that either is technically superior.

## Decision drivers

* **Data integrity:** Enforce relationships and update related records atomically.
* **Querying:** Support joins and structured queries across funding data.
* **Consistency with convention:** Follow the SQL Server convention reflected in
  the service's existing development setup and planned test environments.
* **Operability:** Use a managed Azure service rather than operate a database server.
* **Maintainability:** Prefer a familiar approach without introducing complexity
  that current requirements do not justify, in line with
  [ADR-0013: Adopt architectural principles from Alpha](0013-alpha-principles.md).

## Considered options

* **Relational storage using Azure SQL Database:** Supports the required data
  integrity and query patterns, and follows the existing SQL Server convention.
* **Relational storage using Azure Database for PostgreSQL:** An equally credible
  alternative for the current requirements, with comparable relational
  capabilities and a managed Azure offering. There is no identified requirement
  that justifies departing from the SQL Server convention.
* **Document storage, such as Azure Cosmos DB for NoSQL:** Useful when data is
  naturally accessed as self-contained documents. For this service, modelling
  relationships through embedded or duplicated data would introduce additional
  consistency and query-design work without a demonstrated benefit. This is a
  workload-fit decision, not a claim that document databases lack transactions
  or consistency guarantees.

## Decision outcome

Use a relational data model, hosted in Azure SQL Database, as the service's
primary application database.

Choose SQL Server over PostgreSQL on convention. This aligns with the local
[SQL Server development setup](../../tutorials/getting-started.md) and the MSSQL
database envisaged by
[ADR-0019: Use ephemeral environments for full-stack testing](0019-ephemeral-environments.md).
PostgreSQL remains a valid alternative if future requirements or departmental
conventions provide a reason to revisit the choice.

This decision does not require separate read and write databases for
[CQRS](0016-cqrs.md). Service tiers, capacity and detailed deployment configuration
are outside its scope.

## Consequences

* **Positive:** Relationships, constraints and transactions support consistent
  funding data without application-managed duplication between documents.
* **Positive:** Database choice follows the existing service convention rather
  than introducing another database technology without a clear benefit.
* **Negative:** Schema changes require managed migrations.
* **Negative:** SQL Server-specific features and operational tooling can increase
  the effort of moving to PostgreSQL or another database later.
* **Constraint:** Validate Azure SQL-specific behaviour against Azure SQL;
  local SQL Server and substitute test databases do not prove full compatibility.
