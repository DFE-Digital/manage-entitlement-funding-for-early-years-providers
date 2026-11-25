---
title: "ADR-0021: Content Editing"
---

## Status

Accepted

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

### Option 0: No CMS for now - defer decision

* **Positive:** No time spent implementing CMS, at least initially.
* **Positive:** Make decision when better informed.
* **Positive:** Devs can start development sooner.
* **Positive:** May end up with less work overall.
* **Negative:** Makes some changes harder for content designers to make.
* **Negative:** May end up more work down the line.

### Option 1: Contentful headless CMS

* **Positive:** Gov approved. Team already familiar.
* **Positive:** Gives content people the power to make SOME changes without relying on dev effort
* **Positive:** More lightweight than standard CMS like Umbraco.
* **Positive:** Devs have control of how much freedom/restrictions to give editors.
* **Positive:** Multiple people and roles can be set up if need be.
* **Positive:** Decoupled from GitHub.
* **Positive:** We have similar DfE projects we can reference. See [EY Quals Repo](https://github.com/DFE-Digital/check-an-early-years-qualification)
* **Positive:** Handles translations if we need them.
* **Negative:** Takes initial dev effort to build in.
* **Negative:** Won't reduce burden from logical content changes such as changing the information collected by the forms
* **Negative:** Hard to know right now how useful being able to change content is, and which content is likely to need changing

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

Chosen option: **Option 0:** No CMS for now - defer decision

It is preferable to make a decision once we are better informed. This will come with time and experience, notably through rounds of user research. We can work using an iterative approach where we are able to change tact if and when the need occurs.

We also realised that a lot of the content changes require dev effort anyway. For example, if a content designer wants to add another question so we can collect more data, dev work is needed to implement storing this new piece of information. If a dev is already implementing the logical side of this change, altering the UI is minimal further effort.

Similarly, simpler text-only content changes won't take a dev long to implement and release, especially if we build our pipelines so. Yes this adds friction for the content designer, but we decided seems worthwhile overall, when compared to the effort of implementing a CMS.

The design will likely go through multiple rounds of iterations and changes over the next few months, so it seems like the devs will regularly be updating the form anyway. This minimises effort for the devs to be updating content. Approaching release deadlines, the design should converge into one stable layout, meaning minimal or no requests to change content.

We will need to decide on some form of update mechanism going forwards, as the Local Authorities will need a way of inputting future term  deadlines over the years. We decided that to make this process more secure, it is best to handle this using some form of Admin UI where we can validate user actions, to ensure that something as important as deadline dates aren't entered incorrectly. For this reason, we decided a CMS isn't a good fit for this use case.

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

Continue dev work as is, review semi-regularly to see if a need arises. Listen out for scenarios that may sway our decision relating to implementing a CMS.

Content designers will need to request changes via the devs. As mentioned, the devs will be actively making regular changes to the form anyway so results in minimal impact.

No consequences for dev work. No time needed to spike and implement a CMS for now.

May end up implementing CMS down the line anyway.

Will need to decide on a solution for submitting term deadlines. Can decide this later on.

Unlikely that we will need to support different translations, as it isn't every parent in the UK, but every provider, and one could assume that every Provider must be able to speak enough English to understand. If this isn't the case, we can consider it when making the deferred decision.
