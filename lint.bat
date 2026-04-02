@echo off
setlocal

REM Comprehensive Linting Script
REM 
REM PURPOSE:
REM - Run ALL lint checks when executed (no options or modes)
REM - Output lint failures directly for agent parsing
REM - NO command-line arguments, pretty printing, or colorization
REM - Agents execute this script to identify files needing fixes

set "LINT_ERROR=0"

REM Install npm dependencies
call npm install --silent

REM Create Python virtual environment (for yamllint) if missing
if not exist ".venv\Scripts\activate.bat" (
    python -m venv .venv
)
call .venv\Scripts\activate.bat
pip install -r pip-requirements.txt --quiet --disable-pip-version-check

REM Run spell check
call npx cspell --no-progress --no-color --quiet "**/*.{md,yaml,yml,json,cs,cpp,hpp,h,txt}"
if errorlevel 1 set "LINT_ERROR=1"

REM Run markdownlint check
call npx markdownlint-cli2 "**/*.md"
if errorlevel 1 set "LINT_ERROR=1"

REM Run yamllint check
yamllint .
if errorlevel 1 set "LINT_ERROR=1"

REM Run .NET formatting check (verifies no changes are needed)
dotnet format --verify-no-changes
if errorlevel 1 set "LINT_ERROR=1"

exit /b %LINT_ERROR%
