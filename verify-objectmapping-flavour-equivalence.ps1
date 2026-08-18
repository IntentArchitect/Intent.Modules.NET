<#
.SYNOPSIS
    Verifies that the ObjectMapping.Strict and ObjectMapping.Lenient flavour applications
    generate identical mapping code, apart from the entries governed by Null Path Handling.

.DESCRIPTION
    Spec: object-mapper-hardening, requirement R8.10 (task T6.3).

    Both flavour applications model the same Domain and Services element-for-element
    (R8.2), so their generated Mappings folders must agree once the application-name
    prefix is normalised away. The ONLY permitted differences are the four R1-governed
    initializer entries, where Strict emits the null-forgiving form and Lenient emits
    the null-conditional-plus-default form:

        CouponId, CustomerCity, CouponPercentOff, CouponKind

    Anything else that differs is model drift between the two copies, or a null-handling
    branch leaking into an expression it has no business touching.

    This script lives at the repository root - deliberately outside either application's
    output - so it needs no code-management directive and neither solution depends on
    the other (design D3).

.OUTPUTS
    Exit code 0 when the two flavours are equivalent; 1 when they are not.
#>
[CmdletBinding()]
param(
    [string] $RepositoryRoot = $PSScriptRoot
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$strictRoot  = Join-Path $RepositoryRoot 'Tests/ObjectMapping.Strict/ObjectMapping.Strict.Application/Mappings'
$lenientRoot = Join-Path $RepositoryRoot 'Tests/ObjectMapping.Lenient/ObjectMapping.Lenient.Application/Mappings'

# The four DTO fields whose expression is governed by Null Path Handling (R1.2 / R1.3).
$nullHandlingGovernedFields = @('CouponId', 'CustomerCity', 'CouponPercentOff', 'CouponKind')

function Get-NormalisedLines {
    param([string] $Path)

    # Normalise the flavour name out so the two copies are directly comparable.
    (Get-Content -LiteralPath $Path) -replace 'ObjectMapping\.(Strict|Lenient)', 'ObjectMapping.<Flavour>'
}

function Test-IsGovernedEntry {
    param([string] $Line)

    foreach ($field in $nullHandlingGovernedFields) {
        if ($Line -match "^\s*$([regex]::Escape($field))\s*=") { return $true }
    }
    return $false
}

foreach ($root in @($strictRoot, $lenientRoot)) {
    if (-not (Test-Path -LiteralPath $root)) {
        Write-Host "Mappings folder not found: $root. Run the Software Factory for both flavour applications first." -ForegroundColor Red
        exit 1
    }
}

# -Name yields paths relative to the root directly, which avoids any substring arithmetic
# (and the short-path / mixed-separator traps that come with it).
$strictNames  = @(Get-ChildItem -LiteralPath $strictRoot  -Recurse -File -Filter *.cs -Name | Sort-Object)
$lenientNames = @(Get-ChildItem -LiteralPath $lenientRoot -Recurse -File -Filter *.cs -Name | Sort-Object)

if ($strictNames.Count -eq 0) {
    Write-Host "No mapping files found under $strictRoot - nothing to compare." -ForegroundColor Red
    exit 1
}

$differences = New-Object System.Collections.Generic.List[string]

foreach ($missing in ($strictNames | Where-Object { $lenientNames -notcontains $_ })) {
    $differences.Add("Only in Strict: $missing")
}
foreach ($missing in ($lenientNames | Where-Object { $strictNames -notcontains $_ })) {
    $differences.Add("Only in Lenient: $missing")
}

foreach ($name in ($strictNames | Where-Object { $lenientNames -contains $_ })) {
    $strictLines  = @(Get-NormalisedLines (Join-Path $strictRoot  $name))
    $lenientLines = @(Get-NormalisedLines (Join-Path $lenientRoot $name))

    if ($strictLines.Count -ne $lenientLines.Count) {
        $differences.Add("$name : line count differs (Strict $($strictLines.Count), Lenient $($lenientLines.Count))")
        continue
    }

    for ($i = 0; $i -lt $strictLines.Count; $i++) {
        if ($strictLines[$i] -ceq $lenientLines[$i]) { continue }

        # A difference is permitted only on one of the four Null-Path-Handling-governed entries,
        # and only when BOTH sides are that same entry.
        if ((Test-IsGovernedEntry $strictLines[$i]) -and (Test-IsGovernedEntry $lenientLines[$i])) {
            $strictField  = ($strictLines[$i]  -split '=')[0].Trim()
            $lenientField = ($lenientLines[$i] -split '=')[0].Trim()
            if ($strictField -ceq $lenientField) { continue }
        }

        $differences.Add("$name : line $($i + 1)`n    Strict : $($strictLines[$i].Trim())`n    Lenient: $($lenientLines[$i].Trim())")
    }
}

if ($differences.Count -gt 0) {
    Write-Host "Flavour equivalence FAILED - $($differences.Count) unexpected difference(s):" -ForegroundColor Red
    foreach ($difference in $differences) { Write-Host "  $difference" -ForegroundColor Red }
    Write-Host ""
    Write-Host "Only these initializer entries may differ between flavours: $($nullHandlingGovernedFields -join ', ')"
    exit 1
}

Write-Host "Flavour equivalence OK - $($strictNames.Count) mapping file(s) compared; differences confined to $($nullHandlingGovernedFields -join ', ')." -ForegroundColor Green
exit 0
