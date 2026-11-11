---
title: Contributing
layout: sub-navigation
sectionKey: Tutorials
order: 2
includeInBreadcrumbs: true
eleventyNavigation:
  parent: Tutorials
  key: Contributing
---


## Pull requests and development deployments

Pull requests by team members run validation checks and deploy ephemeral deployments
into the `development` environment, which are torn down when a pull request is closed.

After pull requests are merged to `main`; the integrated code is also deployed into
the `development` environment. In future; this will likely change to a `test` environment.

The infrastructure is currently a draft: each deployment has a resource group and
separate Terraform state, but no web application or deployed-environment tests.

## Pull requests from contributors

External contributions are welcome, but pull requests opened from forks
will not deploy into the shared environment, and must be brought "in-house"
by a team member, while preserving co-authorship.

A maintainer should:

1. Review the fork's exact commits, including workflow changes, Terraform,
   providers, scripts and other executable code.
2. Fetch and cherry-pick the reviewed commits onto a new branch in this
   repository, preserving the contributor's authorship.
3. Push that internal branch and open a replacement PR targeting `main`. Link
   the original PR so the contribution and discussion remain traceable.
4. Close the original fork PR with a link to the replacement.

The internal PR follows the normal automatic validation and deployment flow.
Later commits in the fork do not automatically enter the internal branch; review
and bring each subsequent update across deliberately.

## Code formatting

CI checks the solution against `.editorconfig` after building. From the repository
root, run the same check locally:

```shell
dotnet format Dfe.ManageEntitlementFunding.slnx --verify-no-changes --no-restore
```

This assumes dependencies have already been restored. Any required formatting
changes cause a non-zero exit code and fail the CI build job. To apply fixes locally:

```shell
dotnet format Dfe.ManageEntitlementFunding.slnx
```

The check covers supported .NET formatting rules and analyzer diagnostics with
automatic fixes at warning severity or higher.

## Markdown formatting

Repository markdown is linted in CI. From the repository root, run:

```shell
just lint-markdown
```

Lint errors fail the build without modifying files. To apply automatic fixes
after installing dependencies:

```shell
npm --prefix docs run lint:fix
```

Some errors require manual fixes. The rules in the root
`.markdownlint.json` are also used by the VS Code
markdownlint extension.
