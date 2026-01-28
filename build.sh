#!/usr/bin/env bash
# Build and test SonarMark

set -e  # Exit on error

echo "🔧 Building SonarMark..."
dotnet build --configuration Release

echo "✅ Running tests..."
dotnet test --configuration Release --verbosity normal

echo "✨ Build and test completed successfully!"
