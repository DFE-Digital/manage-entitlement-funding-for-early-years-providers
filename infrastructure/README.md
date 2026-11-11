# Infrastructure draft

This draft adopts the Terraform bootstrap pattern from
[childcare-support-platform](https://github.com/DFE-Digital/childcare-support-platform).
It provisions the remote backend and wires Terraform into GitHub Actions.
It does **not** provision or deploy a web application yet.

## Layout

- `bicep/`: subscription-scoped backend provisioning and environment parameters.
- `scripts/`: temporary GitHub-hosted runner access to state storage.
- `terraform/`: application infrastructure configuration, currently containing
  only provider configuration and an environment output.

## Deployment sequence

The [Terraform Deploy workflow](../.github/workflows/tf-azure-deploy.yml) validates
the configuration on relevant pull requests without Azure access. Every push to
`main`, including a merged pull request, validates and then deploys to the GitHub
`development` environment. Bootstrap runs automatically on **every** deployment:

1. Authenticate with the existing Azure identity using GitHub OIDC.
2. Deploy Bicep to create or update a dedicated backend resource group.
3. Use Bicep outputs to locate the storage account and resource group.
4. Allow the runner's public IPv4 address through the storage firewall.
5. Initialise Terraform with `-upgrade`, plan, and apply the saved plan.
6. Disable public storage access and remove the runner's firewall rule, including
   when Terraform fails.

There is no plan-only option or manual deployment trigger. Planning remains an
internal step so that apply executes the saved plan. Development runs are
serialised and active deployments are not cancelled by subsequent pushes.
Only development is configured.

## Backend resources

Bicep owns these resources separately from Terraform:

- Resource group.
- Zone-redundant storage account and private `tfstate` blob container.
- Blob versioning and 14-day blob/container soft deletion.
- Log Analytics workspace with 90-day retention and blob diagnostics.
- Dedicated VNet, subnet and NSG.
- Blob private endpoint, private DNS zone and VNet link.
- Storage Blob Data Contributor role assignment for the workflow identity.

Shared keys and anonymous blob access remain disabled. Terraform authenticates
with OIDC and Entra ID. Development has its own backend account, using
container `tfstate` and state key `terraform.tfstate`.

The hosted runner uses the temporarily permitted **public blob endpoint**, not
the private endpoint. Bicep deployment and firewall changes use Azure's public
management API. The private endpoint supports clients with connectivity to the
backend VNet; this draft does not create a self-hosted runner or VNet peering.

Cleanup is best-effort at workflow level: runner loss or forced termination can
prevent it from running. If a run is interrupted, verify that public network
access is disabled before rerunning.

## Before the first deployment

1. Replace every `REPLACE_ME` value in
   `bicep/environments/development.params.json` with approved resource names. Storage
   account names must be globally unique, 3-24 lowercase letters/digits.
2. Review the draft VNet/subnet address ranges, UK South location, tags and storage
   SKU against subscription and landing-zone requirements.
3. Use a GitHub environment named `development`, with
   the existing OIDC trust and `AZURE_CLIENT_ID`, `AZURE_TENANT_ID` and
   `AZURE_SUBSCRIPTION_ID` secrets available to each deployment job.
4. Confirm Azure permissions and required resource-provider registrations before
   running. Identity provisioning and permission changes are outside this draft.
5. Merge to `main` to run **Terraform Deploy**. This currently provisions the
   backend and applies the Terraform environment output, not a web application.

The Terraform CLI defaults to `1.15.8` and AzureRM to `5.4.0`, matching the platform
reference. A repository variable `TERRAFORM_VERSION` can override the CLI version.
The deployment uses `terraform init -upgrade`, also following the platform.
Provider versions remain constrained by the configuration.

## Local validation

With Azure CLI/Bicep, Terraform and Bash installed, from the repository root:

```powershell
az bicep build --file infrastructure\bicep\main.bicep --stdout
bash -n infrastructure/scripts/manage-storage-access.sh
terraform -chdir=infrastructure/terraform fmt -check -recursive
terraform -chdir=infrastructure/terraform init -backend=false -input=false
terraform -chdir=infrastructure/terraform validate
```

These checks do not deploy Azure resources or require backend access.
Commit the generated `.terraform.lock.hcl` when establishing or updating provider
dependencies; do not commit local state, plan files or `.terraform/`.

## Differences from the reference

The draft deploys only to development on pushes to `main`. It omits
application-specific provisioning, package promotion and release steps.
Unlike the reference's fixed sleep, storage access is checked with bounded
retries. The runner IP is resolved once for both opening and closing access, and
cleanup disables public access before removing the IP rule. Failures are surfaced
rather than ignored.
