$intent_solution = 'Tests/Intent.Modules.NET.Tests.isln'

# Username and password use the same environment variables as the packager, see:
# https://github.com/garethbaars/Intent/blob/14f6f75be682e8d38e3b2c6c185fe45eac72399a/Intent.IntentArchitect/IntentArchitect.ElectronClient/Intent.IArchitect.Packager.CommandLine/Program.cs#L166
# (NOTE: System.Security.Cryptography.ProtectedData is not available on non-Windows platforms)
$intent_architect_user = $Env:INTENT_PACKAGER_USERNAME
$intent_architect_password = [System.Text.Encoding]::Unicode.GetString([System.Security.Cryptography.ProtectedData]::Unprotect([System.Convert]::FromBase64String($Env:INTENT_PACKAGER_PASSWORD), $null, "CurrentUser"))

$testSln = [xml] (Get-Content "./$($intent_solution)" -Encoding UTF8)

$block = { Param($application, $intent_architect_user, $intent_architect_password, $intent_solution)
	$testsFailed = $false
	$appId = $application.id
	Write-Host ==================================================
	Write-Host Application = $application.name
	Write-Host ==================================================
	$output = intent-cli "$($intent_architect_user)" "$($intent_architect_password)" "$($intent_solution)" "$($appId)"

	if ($LASTEXITCODE -ne 0) {
		$testsFailed = $true
		Write-Error "Changes detected."
	} else {
		Write-Host "No changes detected."
	}
	Write-Host ""

	return $testsFailed
}

$startTime = Get-Date

# Remove all jobs
Get-Job | Remove-Job
$MaxThreads = 4
$count = 0

# Start the jobs. Max 4 jobs running simultaneously.
foreach ($application in $testSln.solution.applications.application) {
	While ($(Get-Job -state running).count -ge $MaxThreads) {
		Start-Sleep -Milliseconds 3
	}

	$count++
	Start-Job -Name "$count of $($testSln.solution.applications.ChildNodes.count) ($($application.name))" -Scriptblock $block -ArgumentList @($application, $intent_architect_user, $intent_architect_password, $intent_solution)
}
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
Write-Output "Total Execution Time: $($executionTime.TotalSeconds) seconds"

if ($testsFailed -eq $true) {
	throw "Template Testing failed. Please review [error] tags."
}