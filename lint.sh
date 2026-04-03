#!/bin/bash

# Comprehensive Linting Script
#
# PURPOSE:
# - Run ALL lint checks when executed (no options or modes)
# - Output lint failures directly for agent parsing
# - NO command-line arguments, pretty printing, or colorization
# - Agents execute this script to identify files needing fixes

lint_error=0

# === PYTHON SECTION ===

# Create python venv if necessary
if [ ! -d ".venv" ]; then
    python -m venv .venv || { lint_error=1; skip_python=1; }
fi

# Activate python venv
if [ "$skip_python" != "1" ]; then
    source .venv/bin/activate || { lint_error=1; skip_python=1; }
fi

# Install python tools
if [ "$skip_python" != "1" ]; then
    pip install -r pip-requirements.txt --quiet --disable-pip-version-check || { lint_error=1; skip_python=1; }
fi

# Run yamllint
if [ "$skip_python" != "1" ]; then
    yamllint . || lint_error=1
fi

# === NPM SECTION ===

# Install npm dependencies
npm install --silent || { lint_error=1; skip_npm=1; }

# Run cspell
if [ "$skip_npm" != "1" ]; then
    npx cspell --no-progress --no-color --quiet "**/*.{md,yaml,yml,json,cs,cpp,hpp,h,txt}" || lint_error=1
fi

# Run markdownlint-cli2
if [ "$skip_npm" != "1" ]; then
    npx markdownlint-cli2 "**/*.md" || lint_error=1
fi

# === DOTNET SECTION ===

# Run dotnet format
dotnet format --verify-no-changes || lint_error=1

# Report result
exit $lint_error
