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

## Markdown & Yaml formatting

Repository markdown and yaml are linted in CI. To run locally, from the repository root run:

```shell
just lint-markdown
just lint-yaml
```

Lint errors fail the build without modifying files. To apply automatic fixes
after installing dependencies:

```shell
npm --prefix docs run lint:md:fix
npm --prefix docs run lint:yaml:fix
```

Some errors require manual fixes. The rules in the root
`.markdownlint.json` are also used by the VS Code
markdownlint extension.
