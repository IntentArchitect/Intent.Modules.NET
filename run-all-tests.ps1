Write-Host "Searching for .sln files..."

$slnFiles = Get-ChildItem Tests/**/*.sln -Recurse -Depth 2 | Where-Object { $_ -NotLike "*previous_output*" }
$count = 0

foreach ($slnFile in $slnFiles) {
    $count++
    Write-Host
    Write-Host "Preparing $count of $($slnFiles.Count): $slnFile..."
	$Env:TF_BUILD = $true
    dotnet test $slnFile

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Exited early as test run failed unexpectantly for $slnFile"
        Pause
        exit
    }
}

Write-Host
Write-Host "All solutions completed tests run."
Pause