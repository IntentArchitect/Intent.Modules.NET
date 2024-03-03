Write-Host "Searching for .sln files..."

$slnFiles = Get-ChildItem Tests/**/*.sln -Recurse -Depth 2 | Where-Object { $_ -NotLike "*previous_output*" }
$count = 0

foreach ($slnFile in $slnFiles) {
    $count++
    Write-Host
    Write-Host "Building $count of $($slnFiles.Count): $slnFile..."
    dotnet build $slnFile --interactive

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Exited early as build failed for $slnFile"
        Pause
        exit
    }
}

Write-Host
Write-Host "All solutions built successfully."
Pause