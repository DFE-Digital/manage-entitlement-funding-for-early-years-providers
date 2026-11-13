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
├─ infrastructure/ -- Bicep Terraform-backend bootstrap and Terraform configuration.
├─ tests/
```

## Documentation

The [technical documentation](https://dfe-digital.github.io/manage-entitlement-funding-for-early-years-providers/) is automatically published and hosted on GitHub Pages.

The documentation site covers:

- [Getting Started Guide](https://dfe-digital.github.io/manage-entitlement-funding-for-early-years-providers/tutorials/getting-started/).
- [Architecture decisions](docs/reference/decisions/README.md).
- [Development deployment how-to](docs/how-to/deploy-development-sites.md).
- [Development deployment reference](docs/reference/development-deployments.md).

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
