#!/usr/bin/env bash
set -euo pipefail

just init "$(node -p "require('node:crypto').randomBytes(32).toString('base64')")"
