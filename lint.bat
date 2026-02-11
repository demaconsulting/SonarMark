@echo off
REM Run all linters for SonarMark (Windows)

echo Checking markdown...
call npx markdownlint-cli2
if %errorlevel% neq 0 exit /b %errorlevel%

echo Checking spelling...
call npx cspell "**/*.{cs,md,json,yaml,yml}" --no-progress
if %errorlevel% neq 0 exit /b %errorlevel%

echo Checking YAML...
yamllint .
if %errorlevel% neq 0 exit /b %errorlevel%

echo Checking code formatting...
dotnet format --verify-no-changes
if %errorlevel% neq 0 exit /b %errorlevel%

echo All linting passed!
