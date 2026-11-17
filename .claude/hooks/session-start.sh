#!/bin/bash
# Prepares a Claude Code cloud session: installs the .NET 10 SDK and Node.js 22, then restores
# backend NuGet packages and frontend npm dependencies. Local sessions skip it.
set -euo pipefail

if [ "${CLAUDE_CODE_REMOTE:-}" != "true" ]; then
  exit 0
fi

cd "$CLAUDE_PROJECT_DIR"

# Ubuntu's package matches backend/global.json (10.0.1xx with rollForward latestFeature).
if ! dotnet --list-sdks 2>/dev/null | grep -q '^10\.'; then
  apt-get update -qq
  DEBIAN_FRONTEND=noninteractive apt-get install -y -qq dotnet-sdk-10.0
fi
echo 'export DOTNET_CLI_TELEMETRY_OPTOUT=1' >> "$CLAUDE_ENV_FILE"
echo 'export DOTNET_NOLOGO=1' >> "$CLAUDE_ENV_FILE"
(cd backend && dotnet restore ModularTemplate.slnx)

# Node.js 22, the version CI uses. nvm is not compatible with `set -eu`, so it runs with both turned off.
export NVM_DIR="${NVM_DIR:-/opt/nvm}"
set +eu
. "$NVM_DIR/nvm.sh"
nvm install --no-progress 22
node_bin="$(dirname "$(nvm which 22)")"
set -eu
export PATH="$node_bin:$PATH"
echo "export PATH=\"$node_bin:\$PATH\"" >> "$CLAUDE_ENV_FILE"
(cd frontend && npm install --no-save --no-audit --no-fund)
