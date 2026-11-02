---
title: "ADR-0017: Publish technical documentation"
---

## Status

Accepted

## Context and problem statement

Technical documentation is stored as Markdown files in the `/docs` directory. This should remain accessible as markdown for developers; but an accessible, branded and searchable documentation site may be more convenient and is required for stakeholders.

## Decision drivers

* **Compatibility** The documentation should continue to work effectively in GitHub.
* **Branding:** Compliance with GOV.UK/DfE styling for public-facing or stakeholder documentation.
* **Maintainability:** Low-friction processes for updating and adding documentation.
* **Discovery:** Built-in navigation and search functionality.
* **Consistency:** Alignment with other DfE Digital projects.

## Considered options

### Option 1: Retain Markdown in the repository (No action)

* **Positive:** No additional tooling or build steps required; zero financial cost.
* **Negative:** Lacks branding, provides poor navigation for non-technical users, and relies on GitHub's native search functionality.

### Option 2: Eleventy (11ty) with the [GOV.UK Eleventy plugin](https://x-govuk.github.io/govuk-eleventy-plugin/)

* **Positive:** Provides a comprehensive GOV.UK-branded documentation site, aligns with DfE Digital standards, and includes built-in navigation and search.
* **Negative:** Requires a Node.js environment for the build process.

Other DfE projects that take this approach:

* [Accessing Childcare Entitlement Checker](https://dfe-digital.github.io/accessing-childcare-entitlement-checker/)
* [Single Unique Identifier](https://dfe-digital.github.io/single-unique-identifier/)
* [Education Benchmarking and Insights](https://dfe-digital.github.io/education-benchmarking-and-insights/)
* [Support for Care Leavers](https://dfe-digital.github.io/care-leavers/)

### Option 3: Other docs generators in use at DfE

* **Positive:** Enables conversion of markdown docs to HTML for publishing, making them accessible
* **Negative:** Middleman and Jekyll are used for dedicated docs sites rather than repo sidecars. No mature, officially supported GOV.UK theme exists for MkDocs; introduces a Python environment dependency.

* [Get To An Answer](https://dfe-digital.github.io/get-to-an-answer/) (MkDocs)
* [SUI Matcher](https://dfe-digital.github.io/SUI_Matcher/) (MkDocs)
* [DfE Technical Guidance](https://technical-guidance.education.gov.uk/) (Middleman)
* [Dfe Architecture](https://dfe-digital.github.io/architecture/) (Jekyll)

## Decision outcome

Chosen option: **Option 2: Eleventy (11ty) with the GOV.UK Eleventy Plugin**.

This option satisfies the requirement for GOV.UK styling using a maintained plugin. It aligns with established DfE Digital standards and provides search, navigation, and branding capabilities.

## Consequences

* **Positive:** Technical documentation matches the styling of other DfE services.
* **Positive:** Publishing to GitHub Pages is automated via GitHub Actions.
* **Negative:** Adds a Node.js dependency to the project's documentation build pipeline.
