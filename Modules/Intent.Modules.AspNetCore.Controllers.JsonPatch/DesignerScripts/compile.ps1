tsc -p ./tsconfig.json

if ($LASTEXITCODE -ne 0) {
    Write-Host "Exited early as build failed for tsconfig.json"
    exit
}