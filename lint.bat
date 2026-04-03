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

REM === PYTHON SECTION ===

REM Create python venv if necessary
if not exist ".venv\Scripts\activate.bat" python -m venv .venv
if errorlevel 1 goto skip_python

REM Activate python venv
call .venv\Scripts\activate.bat
if errorlevel 1 goto skip_python

REM Install python tools
pip install -r pip-requirements.txt --quiet --disable-pip-version-check
if errorlevel 1 goto skip_python

REM Run yamllint
yamllint .
if errorlevel 1 set "LINT_ERROR=1"
goto npm_section

:skip_python
set "LINT_ERROR=1"

REM === NPM SECTION ===

:npm_section

REM Install npm dependencies
call npm install --silent
if errorlevel 1 goto skip_npm

REM Run cspell
call npx cspell --no-progress --no-color --quiet "**/*.{md,yaml,yml,json,cs,cpp,hpp,h,txt}"
if errorlevel 1 set "LINT_ERROR=1"

REM Run markdownlint-cli2
call npx markdownlint-cli2 "**/*.md"
if errorlevel 1 set "LINT_ERROR=1"
goto dotnet_section

:skip_npm
set "LINT_ERROR=1"

REM === DOTNET SECTION ===

:dotnet_section

REM Run dotnet format
dotnet format --verify-no-changes
if errorlevel 1 set "LINT_ERROR=1"

REM Report result
exit /b %LINT_ERROR%
