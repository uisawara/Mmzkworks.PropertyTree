@echo off
echo Running PropWrapSharp benchmarks...

REM Build the project
echo Building project...
dotnet build --configuration Release

REM Run benchmarks
echo Running benchmarks...
dotnet run --configuration Release --project Benchmarks

REM Generate HTML report with Python script
echo Generating HTML report...
python generate_report.py

echo Completed!
echo Result files:
echo   - benchmark-results.json (raw data)
echo   - benchmark-report.html (HTML report)
pause 