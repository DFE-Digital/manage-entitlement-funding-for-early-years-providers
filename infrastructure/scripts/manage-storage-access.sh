#!/usr/bin/env bash
set -euo pipefail

STORAGE_ACCOUNT_NAME="${1:?Storage account name is required}"
RESOURCE_GROUP_NAME="${2:?Resource group name is required}"
ENABLE_ACCESS="${3:?Access mode is required}"
WORKER_IP="${4:?Runner public IPv4 address is required}"

case "$ENABLE_ACCESS" in
  true)
    echo "Enabling state access for runner $WORKER_IP..."
    az storage account network-rule add \
      --account-name "$STORAGE_ACCOUNT_NAME" \
      --resource-group "$RESOURCE_GROUP_NAME" \
      --ip-address "$WORKER_IP" --output none

    az storage account update \
      --name "$STORAGE_ACCOUNT_NAME" \
      --resource-group "$RESOURCE_GROUP_NAME" \
      --public-network-access Enabled --output none

    for attempt in {1..18}; do
      if az storage container show \
        --account-name "$STORAGE_ACCOUNT_NAME" \
        --name tfstate --auth-mode login --output none; then
        exit 0
      fi
      echo "Waiting for storage firewall and role assignment propagation ($attempt/18)..."
      sleep 10
    done
    echo "Error: Terraform state container is still inaccessible." >&2
    exit 1
    ;;
  false)
    echo "Disabling public state access..."
    az storage account update \
      --name "$STORAGE_ACCOUNT_NAME" \
      --resource-group "$RESOURCE_GROUP_NAME" \
      --public-network-access Disabled --output none

    az storage account network-rule remove \
      --account-name "$STORAGE_ACCOUNT_NAME" \
      --resource-group "$RESOURCE_GROUP_NAME" \
      --ip-address "$WORKER_IP" --output none
    ;;
  *)
    echo "Error: Access mode must be true or false: $ENABLE_ACCESS" >&2
    exit 1
    ;;
esac
