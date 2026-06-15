#!/bin/bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

if [ ! -d "node_modules" ]; then
  echo "[INFO] Installing frontend dependencies..."
  npm install
fi

echo "[INFO] Building frontend in CI mode..."
NG_CLI_ANALYTICS=ci CI=1 npm run build:prod

echo "[SUCCESS] Frontend build finished. Output is under dist/assignment-ui"
