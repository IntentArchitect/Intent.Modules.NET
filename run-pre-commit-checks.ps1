param(
    [switch]$Reset
)

if ($Reset) {
    ./PipelineScripts/run-pre-commit-checks.ps1 -Reset
    exit 0
}

./PipelineScripts/run-pre-commit-checks.ps1 -ModulesIsln "Modules/Intent.Modules.NET.isln" -TestsIsln "Tests/Intent.Modules.NET.Tests.isln"

exit 0
