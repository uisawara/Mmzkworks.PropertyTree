# PropWrapSharp Benchmark Execution Script

Write-Host "Running PropWrapSharp benchmarks..." -ForegroundColor Green

# Build the project
Write-Host "Building project..." -ForegroundColor Yellow
dotnet build --configuration Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed" -ForegroundColor Red
    exit 1
}

# Run benchmarks
Write-Host "Running benchmarks..." -ForegroundColor Yellow
dotnet run --configuration Release --project Benchmarks

if ($LASTEXITCODE -ne 0) {
    Write-Host "Benchmark execution failed" -ForegroundColor Red
    exit 1
}

# Generate HTML report with Python script
Write-Host "Generating HTML report..." -ForegroundColor Yellow
python generate_report.py

if ($LASTEXITCODE -ne 0) {
    Write-Host "HTML report generation failed" -ForegroundColor Red
    exit 1
}

Write-Host "Completed!" -ForegroundColor Green
Write-Host "Result files:" -ForegroundColor Cyan
Write-Host "  - benchmark-results.json (raw data)" -ForegroundColor White
Write-Host "  - benchmark-report.html (HTML report)" -ForegroundColor White

# Open HTML report
if (Test-Path "benchmark-report.html") {
    Write-Host "Opening HTML report..." -ForegroundColor Yellow
    Start-Process "benchmark-report.html"
} 