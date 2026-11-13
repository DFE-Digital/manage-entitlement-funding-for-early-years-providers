---
title: "ADR-0018: Content Editing"
---

## Status

Proposed

## Context and problem statement

We want to build the platform in a way that supports the needs of Content Designers and Local Authorities with minimal disruption. This can involve wanting to change words on a page, or needing to upload future term dates.

## Decision drivers

* **Useful:** Low-friction process for relevant parties to make edits.
* **Compliant:** Compliance with GOV.UK standards.
* **Consistency:** Alignment with other DfE Digital projects.
* **Robust:** Will work to a good standard with no or limited downtime.
* **Secure:** Only approved people will be able to access
* **Cost-effective:** Cost doesn't outweigh benefits

## Considered options

### Option 1: Contentful headless CMS - implemented at Beta Private

* **Positive:** Gov approved. Team already familiar.
* **Positive:** Gives content people the power to make changes without relying on dev effort
* **Positive:** More lightweight than standard CMS like Umbraco.
* **Positive:** Devs have control of how much freedom/restrictions to give editors.
* **Positive:** Multiple people and roles can be set up if need be.
* **Positive:** Decoupled from GitHub.
* **Positive:** We have similar DfE projects we can reference. See [EY Quals Repo](https://github.com/DFE-Digital/check-an-early-years-qualification)
* **Negative:** Takes initial dev effort to build in.

### Option 1.5: Contentful headless CMS - implemented at Beta Public

* **Positive:** Same as above.
* **Positive:** Devs can focus on feature building sooner.
* **Negative:** Takes initial dev effort to build in.
* **Negative:** Blocks content designers from making changes easily

### Option 2: Other headless CMS

* **Positive:** Might be just as good, or better than Contentful
* **Negative:** Isn't the standard choice for Gov work.

### Option 3: GitHub JSON file

* **Positive:** Gives content people the power to make changes without relying on dev effort
* **Positive:** Less cost on dev work
* **Positive:** Can get going with this solution sooner
* **Positive:** Can build in a generic way that means that work to integrate with CMS in the future, isn't too hard
* **Positive:** Content Designer seemed happy with this solution "if need be"
* **Negative:** Likely will need to implement CMS at the end anyway, duplicated effort
* **Negative:** Not a standard practice
* **Negative:** Might be confusing for others to use
* **Negative:** Might require GitHub account to be created and added

## Decision outcome

(Not yet) Chosen option: **Option 1:** Contentful headless CMS.

From talking to the Content Designer, and a developer on another DfE team, it seems like changes are likely to be requested more often than initially expected for a transactional service. For this reason, we can reduce blockers for content designers by giving them their preferred interface.

We will likely have to implement a CMS in the end anyway, as the Local Authorities will need a way of inputting future term date deadlines, going forward throughout the years. If we need to spend effort implementing it at the end anyway, we may as well save effort and not implement a temporary, simpler system, earlier on. Build one good thing once, rather than build 2 systems.

We can take inspiration from the existing DfE team ([EY Quals Repo](https://github.com/DFE-Digital/check-an-early-years-qualification)) to speed up dev set up. For example, the [HyperLinkRenderer.cs](https://github.com/DFE-Digital/check-an-early-years-qualification/blob/main/src/Dfe.EarlyYearsQualification.Content/RichTextParsing/Renderers/HyperlinkRenderer.cs) just wraps an existing `<a ... />` from Contentful with the Government class. We will be using the same Government components so we can use the same definitions.

### What content is likely to change

* The submission deadline dates every year going forwards
* Descriptions that explain more information around a questions
* Intro screen explanation
* Headings
* Order of questions
* Wording of question
* Order of answers
* Wording of answers
* Number of answers
* Table headings / structure when viewing submissions / children

### What content is not likely to use Contentful to change

* Submission process
* Number of questions - by the time the team is happy to start development due to enough design and user research iterations, the core form shouldn't change too much
* Sign in logic
* Eligibility checker logic
* Consumables / extra charges - will change through the Provider Admin settings instead
* Provider sign up

## Consequences

Devs will need to spend time setting up a new Contentful instance, including learning how it works, and creating tickets to document the work

Content Designers (and anyone else identified) will be able to edit the web UI from really early on without having to wait for a later phase of the project

Devs will have fewer distractions with no need to edit content once Contentful is in place
