# Manage entitlement funding for early-years providers

A service that allows authenticated early years providers to manage the data
they need to submit claims to local authorities for their provision of statutory
childcare entitlements.

## Project structure

```text
manage-entitlement-funding-for-early-years-providers/
├─ docs/ -- Technical documentation, including architecture decisions under reference/decisions/
├─ prototypes/ -- Proofs of concept and prototype implementations
├─ src/
├─ terraform/ - Terraform project used to implement the Azure infrastructure as code.
├─ tests/
```

## Documentation

The [technical documentation](https://dfe-digital.github.io/manage-entitlement-funding-for-early-years-providers/) is automatically published and hosted on GitHub Pages.

The documentation site covers:

- [Getting Started Guide](https://dfe-digital.github.io/manage-entitlement-funding-for-early-years-providers/tutorials/getting-started/).
- [Architecture decisions](docs/reference/decisions/README.md).

### Previewing the documentation

With Node.js 22.16 or later installed:

```powershell
cd docs
npm ci --ignore-scripts
npm start
```

Open the local URL printed by Eleventy. Markdown lives directly under `docs/`;
`npm run build` writes generated output to `artifacts/docs/` in the repository
root. Dependencies remain in `docs/node_modules/`. Both directories are ignored
by Git.

Documentation builds also lint all repository Markdown, including READMEs.
Run `just lint-markdown` to check without building the site, or
`npm --prefix docs run lint:fix` to apply automatic fixes after installing
dependencies. See the [contributing guide](docs/tutorials/contributing.md) for
details.
