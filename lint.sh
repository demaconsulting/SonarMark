#!/usr/bin/env bash
# Run all linters for SonarMark

set -e  # Exit on error

echo "📝 Checking markdown..."
npx markdownlint-cli2

echo "🔤 Checking spelling..."
npx cspell "**/*.{cs,md,json,yaml,yml}" --no-progress

echo "📋 Checking YAML..."
yamllint .

echo "🎨 Checking code formatting..."
dotnet format --verify-no-changes

echo "✨ All linting passed!"
