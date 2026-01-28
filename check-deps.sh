#!/usr/bin/env bash
# Check dependencies for updates and vulnerabilities

set -e  # Exit on error

echo "🔍 Checking for outdated dependencies..."
dotnet list package --outdated

echo ""
echo "🔒 Checking for vulnerable dependencies..."
dotnet list package --vulnerable --include-transitive

echo ""
echo "✨ Dependency check completed!"
