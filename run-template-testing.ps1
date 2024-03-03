$intent_solution = 'Tests/Intent.Modules.NET.Tests.isln'

# This uses a Windows-based secure way to store and retrieve your credentials to execute the intent-cli tool.
# To encode and encrypt your password run the following Powershell script:
# $encodedEncryptedPassword = [System.Convert]::ToBase64String([System.Security.Cryptography.ProtectedData]::Protect([System.Text.Encoding]::Unicode.GetBytes("Your password"), $null, "CurrentUser"))
$intent_architect_user = $Env:INTENT_PACKAGER_USERNAME
$intent_architect_password = [System.Text.Encoding]::Unicode.GetString([System.Security.Cryptography.ProtectedData]::Unprotect([System.Convert]::FromBase64String($Env:INTENT_PACKAGER_PASSWORD), $null, "CurrentUser"))

$testSln = [xml] (Get-Content "./$($intent_solution)" -Encoding UTF8)

$block = { Param($application, $intent_architect_user, $intent_architect_password, $intent_solution)
	$testsFailed = $false
	$appId = $application.id
	$output = intent-cli "$($intent_architect_user)" "$($intent_architect_password)" "$($intent_solution)" "$($appId)"

	if ($LASTEXITCODE -ne 0) {
		$testsFailed = $true
		Write-Error "$($application.name): Changes detected."
	} else {
		Write-Host "$($application.name): No changes detected."
	}

	return $testsFailed
}

$startTime = Get-Date

# Remove all jobs
Get-Job | Remove-Job
$MaxThreads = [Math]::Ceiling(([int]$Env:NUMBER_OF_PROCESSORS) * 0.75)
$count = 0

Write-Host "Starting test projects:"
Write-Host ""

# Start the jobs. Max 4 jobs running simultaneously.
foreach ($application in $testSln.solution.applications.application | Sort-Object -Property name) {
	While ($(Get-Job -state running).count -ge $MaxThreads) {
		Start-Sleep -Milliseconds 3
	}

	$count++
	$job = Start-Job -Name "$count of $($testSln.solution.applications.ChildNodes.count) ($($application.name))" -Scriptblock $block -ArgumentList @($application, $intent_architect_user, $intent_architect_password, $intent_solution)
	Write-Host "$($job.Name)"
}

Write-Host ""
Write-Host "Outcome:"
Write-Host ""

# Wait for all jobs to finish.
While ($(Get-Job -State Running).count -gt 0) {
	start-sleep 1
}

$testsFailed = $false

# Get information from each job.
foreach ($job in Get-Job) {
	$output = Receive-Job $job
	$testsFailed = $testsFailed -or $output
}
# Remove all jobs created.
Get-Job | Remove-Job

$endTime = Get-Date
$executionTime = $endTime - $startTime

Write-Host ""
Write-Output "Total Execution Time: $($executionTime.TotalSeconds) seconds"

if ($testsFailed -eq $true) {
	Write-Host ""
	Write-Error "Template Testing failed. Look at the projects that have changes and then run Intent UI to inspect those changes."
	$LASTEXITCODE = 1
}