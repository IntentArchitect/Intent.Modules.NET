$tsconfig = Join-Path $PSScriptRoot 'tsconfig.json'
tsc -p $tsconfig

if ($LASTEXITCODE -ne 0) {
    Write-Host "Exited early as build failed for $tsconfig"
    exit
}
