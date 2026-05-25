#Requires -Version 7
<#
.SYNOPSIS
    Wires up tool-specific paths to the canonical .agents source of truth.

.DESCRIPTION
    The repo keeps all AI-tooling content (skills, instructions, prompts) in
    a single source of truth at <repo>/.agents. This script creates the
    folder links each tool expects:

        .claude/skills        -> .agents/skills          (Claude Code)
        .github/instructions  -> .agents/instructions    (GitHub / VS Code Copilot)
        .github/prompts       -> .agents/prompts         (GitHub / VS Code Copilot)
        Modules/.agents       -> .agents                 (Intent Architect / existing path refs)

    On Windows the links are directory junctions (no admin/dev-mode required).
    On macOS/Linux they are symbolic links.

    The script is idempotent: links that already point at the correct source
    are left alone. Real folders at a target path are never overwritten.

.PARAMETER DryRun
    Print actions without performing them.

.PARAMETER Force
    Replace links that point to a stale target. Refuses to replace real folders.

.PARAMETER Verify
    Check-only mode. Exits non-zero if any mapping is missing, stale, or blocked.
#>
[CmdletBinding()]
param(
    [switch]$DryRun,
    [switch]$Force,
    [switch]$Verify
)

$ErrorActionPreference = 'Stop'

$RepoRoot   = (Resolve-Path "$PSScriptRoot/..").Path
$AgentsRoot = Join-Path $RepoRoot '.agents'

if (-not (Test-Path $AgentsRoot)) {
    Write-Error "Source of truth '.agents/' not found at repo root: $AgentsRoot"
    exit 1
}

# Edit this table to add or change links. Each entry maps a real folder
# under .agents to the path a tool expects.
$Mappings = @(
    @{ Source = Join-Path $AgentsRoot 'skills';       Target = Join-Path $RepoRoot '.claude/skills' }
    @{ Source = Join-Path $AgentsRoot 'instructions'; Target = Join-Path $RepoRoot '.github/instructions' }
    @{ Source = Join-Path $AgentsRoot 'prompts';      Target = Join-Path $RepoRoot '.github/prompts' }
    @{ Source = $AgentsRoot;                          Target = Join-Path $RepoRoot 'Modules/.agents' }
)

function Resolve-Absolute {
    param([string]$Path)
    try { return (Resolve-Path -LiteralPath $Path -ErrorAction Stop).Path.TrimEnd('\', '/') }
    catch { return $null }
}

function Get-LinkTargetAbsolute {
    param([System.IO.FileSystemInfo]$Item)
    $raw = $Item.Target
    if (-not $raw) { return $null }
    $first = if ($raw -is [array]) { $raw[0] } else { $raw }
    if ([System.IO.Path]::IsPathRooted($first)) {
        return (Resolve-Absolute $first)
    }
    # Relative link target — resolve against the link's parent directory.
    $parent = Split-Path $Item.FullName -Parent
    return (Resolve-Absolute (Join-Path $parent $first))
}

function Get-LinkState {
    param([string]$TargetPath, [string]$ExpectedSource)
    if (-not (Test-Path -LiteralPath $TargetPath)) { return 'Missing' }
    $item = Get-Item -LiteralPath $TargetPath -Force
    if ($item.LinkType) {
        $actual   = Get-LinkTargetAbsolute -Item $item
        $expected = Resolve-Absolute $ExpectedSource
        if ($actual -and $expected -and ($actual -eq $expected)) { return 'CorrectLink' }
        return 'StaleLink'
    }
    if ($item.PSIsContainer) { return 'RealFolder' } else { return 'RealFile' }
}

function New-FolderLink {
    param([string]$Source, [string]$Target)
    $parent = Split-Path $Target -Parent
    if (-not (Test-Path -LiteralPath $parent)) {
        if ($DryRun) { Write-Host "    [dry-run] mkdir $parent" }
        else { New-Item -ItemType Directory -Path $parent -Force | Out-Null }
    }
    if ($DryRun) {
        Write-Host "    [dry-run] link $Target -> $Source"
        return
    }
    if ($IsWindows) {
        New-Item -ItemType Junction -Path $Target -Target $Source | Out-Null
    } else {
        New-Item -ItemType SymbolicLink -Path $Target -Target $Source | Out-Null
    }
}

function Format-Rel {
    param([string]$Path)
    return $Path.Substring($RepoRoot.Length).TrimStart('\', '/').Replace('\', '/')
}

Write-Host "Repo root  : $RepoRoot"
Write-Host "Source     : $AgentsRoot"
Write-Host ""

$exitCode = 0
foreach ($m in $Mappings) {
    $relTarget = Format-Rel $m.Target
    $relSource = Format-Rel $m.Source
    $state     = Get-LinkState -TargetPath $m.Target -ExpectedSource $m.Source

    switch ($state) {
        'Missing' {
            if ($Verify) {
                Write-Host "MISSING : $relTarget -> $relSource"
                $exitCode = 1
            } else {
                Write-Host "CREATE  : $relTarget -> $relSource"
                New-FolderLink -Source $m.Source -Target $m.Target
            }
        }
        'CorrectLink' {
            Write-Host "OK      : $relTarget -> $relSource"
        }
        'StaleLink' {
            if ($Verify) {
                Write-Host "STALE   : $relTarget (re-run setup.ps1 -Force to replace)"
                $exitCode = 1
            } elseif ($Force) {
                Write-Host "REPLACE : $relTarget -> $relSource"
                if (-not $DryRun) { Remove-Item -LiteralPath $m.Target -Force }
                New-FolderLink -Source $m.Source -Target $m.Target
            } else {
                Write-Host "STALE   : $relTarget (re-run with -Force to replace)"
                $exitCode = 1
            }
        }
        'RealFolder' {
            Write-Host "BLOCKED : $relTarget exists as a real folder. Move or remove it manually."
            $exitCode = 1
        }
        'RealFile' {
            Write-Host "BLOCKED : $relTarget exists as a real file. Move or remove it manually."
            $exitCode = 1
        }
    }
}

Write-Host ""
if ($exitCode -eq 0) { Write-Host "Done." }
else                 { Write-Host "Finished with issues (exit $exitCode). See messages above." }

exit $exitCode
