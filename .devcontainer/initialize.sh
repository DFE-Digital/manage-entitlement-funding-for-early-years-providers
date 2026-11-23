#!/usr/bin/env sh
set -eu

cd "$(dirname "$0")"

if [ ! -f .env ]; then
    umask 077
    password="$(openssl rand -hex 32)"
    printf 'MSSQL_SA_PASSWORD=Aa1!%s\n' "$password" > .env
fi
