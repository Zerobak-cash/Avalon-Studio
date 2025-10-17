#!/usr/bin/env bash
set -euo pipefail
echo "Scanning for duplicate Mono.Debugging.Evaluation stubs..."

found=$(grep -R --line-number "namespace Mono.Debugging.Evaluation" -n . || true)
if [ -z "$found" ]; then
  echo "No evaluation namespace definitions found elsewhere."
  exit 0
fi

echo "Found definitions. Listing files:"
echo "$found"

# Disable any CompatStubs.cs leftover to avoid duplicate types
target="debugger-libs/Mono.Debugging/Mono.Debugging.Evaluation/CompatStubs.cs"
if [ -f "$target" ]; then
  echo "Disabling $target to avoid duplicates (moving to .disabled)"
  mv "$target" "$target.disabled"
  echo "Backed up to $target.disabled"
else
  echo "No CompatStubs.cs to disable."
fi

echo "Done."
