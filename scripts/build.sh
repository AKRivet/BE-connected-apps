#!/usr/bin/env bash
set -euo pipefail

DOTNET="${DOTNET_ROOT:-$HOME/.dotnet}/dotnet"
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

echo "==> Restoring packages"
"$DOTNET" restore "$ROOT"

echo "==> Building (Release)"
"$DOTNET" build "$ROOT" --configuration Release --no-restore

echo "==> Running tests"
"$DOTNET" test "$ROOT" --configuration Release --no-build \
  --logger "trx;LogFileName=TestResults.trx" \
  --results-directory "$ROOT/TestResults"

echo "==> Done"
