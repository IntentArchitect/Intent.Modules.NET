#Requires -Version 7.0
<#
.SYNOPSIS
    Inventories the NuGet packages that Intent.Modules.NET modules install into consumer applications, and finds
    the latest version of each on nuget.org per target framework.

.DESCRIPTION
    Read-only helper for the `module-nuget-bump` skill (.claude/skills/module-nuget-bump/SKILL.md). It never
    edits a module: every change goes through the Module Builder designer.

    It parses every generated `Modules/*/NugetPackages.cs` for the package names, their version per target
    framework, the `locked` flags and the NuGet dependencies, then queries the nuget.org v3 registration API
    using the same rules as `Intent.NuGetReferenceUpdater`'s NuGetApi.cs, so results do not change silently:

      - the latest stable version only; prerelease versions are considered only when no stable version exists;
        unlisted versions are ignored;
      - for each of netstandard2.0, netstandard2.1 and net6.0 to net10.0, the highest version with a dependency
        group targeting exactly that framework; when none matches, an `Any` dependency group, and failing that
        the latest version with no dependencies;
      - a later framework that would resolve to a lower version than an earlier one is pruned.

    Locks are checked against each other through nuget.org dependency trees. A locked row is offered the newest
    version in its major line whose dependencies, resolved transitively for each consumer framework the row
    serves, need no package the module locks on that framework above its lock; newer versions that would are
    reported as skipped. Every row's resulting version is checked the same way, so a row already in the model
    that breaks a lock (and would fail the consumer's restore with NU1605) is reported too.

    Registrations inside hand-written `IntentIgnore` blocks (for example AutoMapper's `UsePreCommercialVersion`
    branch) are reported as "manual" and never proposed for an automatic update.

    It writes only under the output directory (default `<repo>/.nuget-bump/`):

      candidates.json  The inventory, the proposed row actions and the consumer-visible changes per module.
      report.md        The inventory as markdown. On a re-run only the block between the inventory markers is
                       replaced, so the review notes added below it survive.
      cache/           nuget.org lookups, reused for -CacheMaxAgeHours.

.PARAMETER RepoRoot
    The Intent.Modules.NET repository root. Defaults to the git top level of this script's folder.

.PARAMETER ModuleFilter
    Wildcards matched against a module's folder name or its .imodspec id. Defaults to every module.

.PARAMETER OutputDirectory
    Where to write the outputs. Defaults to `<RepoRoot>/.nuget-bump`.

.PARAMETER CacheMaxAgeHours
    How long a cached nuget.org lookup is reused. 0 always queries nuget.org.

.PARAMETER ThrottleLimit
    How many packages are looked up on nuget.org in parallel.

.EXAMPLE
    pwsh .claude/skills/module-nuget-bump/scripts/Get-NuGetBumpCandidates.ps1

.EXAMPLE
    pwsh .claude/skills/module-nuget-bump/scripts/Get-NuGetBumpCandidates.ps1 -ModuleFilter 'Intent.EntityFrameworkCore*'
#>
[CmdletBinding()]
param(
    [string]$RepoRoot,
    [string[]]$ModuleFilter = @('*'),
    [string]$OutputDirectory,
    [int]$CacheMaxAgeHours = 12,
    [int]$ThrottleLimit = 8
)

$ErrorActionPreference = 'Stop'

# `pwsh -File` passes `-ModuleFilter a,b` as one string.
$ModuleFilter = @($ModuleFilter | ForEach-Object { $_ -split ',' } | ForEach-Object { $_.Trim().Trim("'", '"') } | Where-Object { $_ })

# Shared by the main script and the parallel lookup runspaces, which cannot see functions defined here.
$helperSource = @'
function Get-SupportedFrameworks {
    # The frameworks NuGetApi.cs looks up, with the switch arm Module Builder generates for each row.
    @(
        [pscustomobject]@{ Tfm = 'netstandard2.0'; Name = '.NETStandard,Version=v2.0'; Major = 2; Minor = 0; Order = [version]'2.0' }
        [pscustomobject]@{ Tfm = 'netstandard2.1'; Name = '.NETStandard,Version=v2.1'; Major = 2; Minor = 1; Order = [version]'2.1' }
        [pscustomobject]@{ Tfm = 'net6.0'; Name = '.NETCoreApp,Version=v6.0'; Major = 6; Minor = 0; Order = [version]'6.0' }
        [pscustomobject]@{ Tfm = 'net7.0'; Name = '.NETCoreApp,Version=v7.0'; Major = 7; Minor = 0; Order = [version]'7.0' }
        [pscustomobject]@{ Tfm = 'net8.0'; Name = '.NETCoreApp,Version=v8.0'; Major = 8; Minor = 0; Order = [version]'8.0' }
        [pscustomobject]@{ Tfm = 'net9.0'; Name = '.NETCoreApp,Version=v9.0'; Major = 9; Minor = 0; Order = [version]'9.0' }
        [pscustomobject]@{ Tfm = 'net10.0'; Name = '.NETCoreApp,Version=v10.0'; Major = 10; Minor = 0; Order = [version]'10.0' }
    )
}

function Get-AnyFramework {
    [pscustomobject]@{ Tfm = 'any'; Name = 'Any,Version=v0.0'; Major = 0; Minor = 0; Order = [version]'0.0' }
}

function ConvertTo-VersionParts([string]$Version) {
    $value = $Version.Split('+')[0]
    $dash = $value.IndexOf('-')
    $release = if ($dash -ge 0) { $value.Substring($dash + 1) } else { '' }
    $core = if ($dash -ge 0) { $value.Substring(0, $dash) } else { $value }
    $numbers = [System.Collections.Generic.List[long]]::new()
    foreach ($part in $core.Split('.')) {
        $number = 0L
        [void][long]::TryParse($part, [ref]$number)
        $numbers.Add($number)
    }
    while ($numbers.Count -lt 4) { $numbers.Add(0L) }
    [pscustomobject]@{ Numbers = $numbers.ToArray(); Release = $release }
}

# SemVer 2 precedence, with NuGet's optional fourth (revision) component. Returns -1, 0 or 1.
function Compare-NuGetVersion([string]$A, [string]$B) {
    $x = ConvertTo-VersionParts $A
    $y = ConvertTo-VersionParts $B
    for ($i = 0; $i -lt 4; $i++) {
        if ($x.Numbers[$i] -ne $y.Numbers[$i]) { return [Math]::Sign($x.Numbers[$i] - $y.Numbers[$i]) }
    }
    if ($x.Release -eq $y.Release) { return 0 }
    if (-not $x.Release) { return 1 }
    if (-not $y.Release) { return -1 }
    $xLabels = $x.Release.Split('.')
    $yLabels = $y.Release.Split('.')
    for ($i = 0; $i -lt [Math]::Min($xLabels.Count, $yLabels.Count); $i++) {
        $xNumber = 0L
        $yNumber = 0L
        $xIsNumber = [long]::TryParse($xLabels[$i], [ref]$xNumber)
        $yIsNumber = [long]::TryParse($yLabels[$i], [ref]$yNumber)
        if ($xIsNumber -and $yIsNumber) {
            if ($xNumber -ne $yNumber) { return [Math]::Sign($xNumber - $yNumber) }
            continue
        }
        if ($xIsNumber) { return -1 }
        if ($yIsNumber) { return 1 }
        $comparison = [string]::Compare($xLabels[$i], $yLabels[$i], [StringComparison]::OrdinalIgnoreCase)
        if ($comparison -ne 0) { return [Math]::Sign($comparison) }
    }
    return [Math]::Sign($xLabels.Count - $yLabels.Count)
}

function Test-PrereleaseVersion([string]$Version) {
    $Version.Split('+')[0].Contains('-')
}

# Normalises a registration dependency group's targetFramework to the short form used here. A group with no
# framework is NuGet's "Any" framework. Platform-specific frameworks (net8.0-windows) are left as they are, so
# they never match, the same as NuGetFramework equality in NuGetApi.cs.
function ConvertTo-ShortFramework([string]$TargetFramework) {
    if ([string]::IsNullOrWhiteSpace($TargetFramework) -or $TargetFramework -ieq 'any') { return 'any' }
    $value = $TargetFramework.Trim().ToLowerInvariant()
    if ($value -match '^\.?netstandard(\d+)\.(\d+)$') { return "netstandard$($Matches[1]).$($Matches[2])" }
    if ($value -match '^(?:net|\.netcoreapp)(\d+)\.(\d+)$' -and [int]$Matches[1] -ge 5) { return "net$($Matches[1]).$($Matches[2])" }
    return $value
}

# The lower bound of a NuGet version range, as NuGetApi.cs takes `VersionRange.MinVersion`.
function Get-RangeMinVersion([string]$Range) {
    if ([string]::IsNullOrWhiteSpace($Range)) { return $null }
    $value = $Range.Trim()
    if ($value -notmatch '^[\[\(]') { return $value }
    $minimum = $value.Trim('[', ']', '(', ')').Split(',')[0].Trim()
    if ($minimum) { $minimum } else { $null }
}

function Invoke-NuGetRequest([string]$Uri, [switch]$Raw) {
    for ($attempt = 1; ; $attempt++) {
        try {
            if ($Raw) { return Invoke-WebRequest -Uri $Uri -TimeoutSec 60 }
            return Invoke-RestMethod -Uri $Uri -TimeoutSec 60
        }
        catch {
            $status = $_.Exception.Response.StatusCode.value__
            if ($status -eq 404 -or $attempt -ge 3) { throw }
            Start-Sleep -Seconds (2 * $attempt)
        }
    }
}

function Select-LatestPerFramework($Entries, $Frameworks, [switch]$ForceOne) {
    $result = [System.Collections.Generic.List[object]]::new()
    foreach ($framework in $Frameworks) {
        $best = $null
        foreach ($entry in $Entries) {
            if (-not $entry.Groups.ContainsKey($framework.Tfm)) { continue }
            if (-not $best -or (Compare-NuGetVersion $entry.Version $best.Version) -gt 0) { $best = $entry }
        }
        if ($best) {
            $result.Add([pscustomobject]@{
                    tfm           = $framework.Tfm
                    frameworkName = $framework.Name
                    order         = $framework.Order
                    version       = $best.Version
                    dependencies  = @($best.Groups[$framework.Tfm])
                })
        }
    }

    if ($ForceOne -and $result.Count -eq 0) {
        $latest = $null
        foreach ($entry in $Entries) {
            if (-not $latest -or (Compare-NuGetVersion $entry.Version $latest.Version) -gt 0) { $latest = $entry }
        }
        $any = Get-AnyFramework
        $result.Add([pscustomobject]@{ tfm = $any.Tfm; frameworkName = $any.Name; order = $any.Order; version = $latest.Version; dependencies = @() })
    }

    # Ascending by framework, then drop a later framework that would get a lower version than an earlier one:
    # MediatR 12.4.1 targets netstandard2.0 and 11.1.1 is the last to target netstandard2.1, so netstandard2.1
    # consumers are better served by the netstandard2.0 row.
    $sorted = [System.Collections.Generic.List[object]]::new()
    foreach ($item in ($result | Sort-Object order)) { $sorted.Add($item) }
    $i = 0
    while ($i -lt $sorted.Count - 1) {
        if ($sorted[$i].order -lt $sorted[$i + 1].order -and (Compare-NuGetVersion $sorted[$i].version $sorted[$i + 1].version) -gt 0) {
            $sorted.RemoveAt($i + 1)
        }
        else {
            $i++
        }
    }
    $sorted.Reverse()
    $sorted | Select-Object tfm, frameworkName, version, dependencies
}

function Get-NuGetPackageLatest([string]$PackageId) {
    $lowerId = $PackageId.ToLowerInvariant()
    try {
        $index = Invoke-NuGetRequest "https://api.nuget.org/v3/registration5-gz-semver2/$lowerId/index.json"
    }
    catch {
        $status = $_.Exception.Response.StatusCode.value__
        $message = if ($status -eq 404) { 'Not found on nuget.org.' } else { $_.Exception.Message }
        return [pscustomobject]@{ id = $PackageId; found = $false; error = $message }
    }

    $entries = [System.Collections.Generic.List[object]]::new()
    $newest = $null
    foreach ($page in $index.items) {
        $items = if ($page.items) { $page.items } else { (Invoke-NuGetRequest $page.'@id').items }
        foreach ($item in $items) {
            $catalogEntry = $item.catalogEntry
            $deprecation = $null
            if ($catalogEntry.deprecation) {
                $deprecation = [pscustomobject]@{
                    reasons          = @($catalogEntry.deprecation.reasons)
                    message          = $catalogEntry.deprecation.message
                    alternatePackage = $catalogEntry.deprecation.alternatePackage.id
                }
            }
            # Listed or not: when every version is unlisted, the newest one's deprecation says why.
            if (-not $newest -or (Compare-NuGetVersion $catalogEntry.version $newest.Version) -gt 0) {
                $newest = [pscustomobject]@{ Version = $catalogEntry.version; Deprecation = $deprecation }
            }
            if ($catalogEntry.listed -eq $false) { continue }
            $groups = @{}
            foreach ($group in @($catalogEntry.dependencyGroups)) {
                if ($null -eq $group) { continue }
                $tfm = ConvertTo-ShortFramework $group.targetFramework
                if ($groups.ContainsKey($tfm)) { continue }
                $groups[$tfm] = @(@($group.dependencies) | Where-Object { $_ } | ForEach-Object {
                        [pscustomobject]@{ id = $_.id; version = (Get-RangeMinVersion $_.range) }
                    })
            }
            $entries.Add([pscustomobject]@{ Version = $catalogEntry.version; Groups = $groups; ProjectUrl = $catalogEntry.projectUrl; Deprecation = $deprecation })
        }
    }

    $candidates = @($entries | Where-Object { -not (Test-PrereleaseVersion $_.Version) })
    $usedPrerelease = $false
    if ($candidates.Count -eq 0) {
        $candidates = @($entries)
        $usedPrerelease = $true
    }
    if ($candidates.Count -eq 0) {
        $message = 'No listed versions on nuget.org.'
        if ($newest.Deprecation) {
            $message += " Deprecated ($($newest.Deprecation.reasons -join ', '))"
            if ($newest.Deprecation.alternatePackage) { $message += ", replaced by $($newest.Deprecation.alternatePackage)" }
            $message += '.'
        }
        return [pscustomobject]@{ id = $PackageId; found = $false; error = $message; deprecation = $newest.Deprecation }
    }

    $perFramework = @(Select-LatestPerFramework $candidates (Get-SupportedFrameworks))
    if ($perFramework.Count -eq 0) {
        $perFramework = @(Select-LatestPerFramework $candidates @(Get-AnyFramework) -ForceOne)
    }

    $latestOverall = $null
    foreach ($item in $perFramework) {
        if (-not $latestOverall -or (Compare-NuGetVersion $item.version $latestOverall) -gt 0) { $latestOverall = $item.version }
    }
    $latestEntry = $candidates | Where-Object Version -eq $latestOverall | Select-Object -First 1

    # The newest version per major, so a locked row can still be offered a patch within its locked line.
    $newestPerMajor = @{}
    foreach ($entry in $candidates) {
        $major = "$((ConvertTo-VersionParts $entry.Version).Numbers[0])"
        if (-not $newestPerMajor.ContainsKey($major) -or (Compare-NuGetVersion $entry.Version $newestPerMajor[$major].Version) -gt 0) {
            $newestPerMajor[$major] = $entry
        }
    }
    $latestByMajor = [ordered]@{}
    foreach ($major in $newestPerMajor.Keys | Sort-Object { [int]$_ }) {
        $latestByMajor[$major] = [pscustomobject]@{ version = $newestPerMajor[$major].Version; dependencies = [pscustomobject]$newestPerMajor[$major].Groups }
    }

    # The repository URL and release notes are only in the .nuspec.
    $repositoryUrl = $null
    $releaseNotes = $null
    try {
        $lowerVersion = $latestOverall.ToLowerInvariant()
        $response = Invoke-NuGetRequest "https://api.nuget.org/v3-flatcontainer/$lowerId/$lowerVersion/$lowerId.nuspec" -Raw
        $content = $response.Content
        if ($content -is [byte[]]) { $content = [System.Text.Encoding]::UTF8.GetString($content) }
        [xml]$nuspec = $content.TrimStart([char]0xFEFF)
        $repositoryUrl = $nuspec.package.metadata.repository.url
        $releaseNotes = "$($nuspec.package.metadata.releaseNotes)".Trim()
        if ($releaseNotes.Length -gt 500) { $releaseNotes = $releaseNotes.Substring(0, 500) + '…' }
        if (-not $releaseNotes) { $releaseNotes = $null }
    }
    catch {
        # Optional metadata; the lookup result stands without it.
    }

    [pscustomobject]@{
        id             = $PackageId
        found          = $true
        usedPrerelease = $usedPrerelease
        latestOverall  = $latestOverall
        nugetUrl       = "https://www.nuget.org/packages/$PackageId/$latestOverall"
        projectUrl     = $latestEntry.ProjectUrl
        repositoryUrl  = $repositoryUrl
        releaseNotes   = $releaseNotes
        deprecation    = $latestEntry.Deprecation
        latestByMajor  = [pscustomobject]$latestByMajor
        latest         = $perFramework
    }
}
'@

. ([scriptblock]::Create($helperSource))

#region Paths

if (-not $RepoRoot) {
    $RepoRoot = (& git -C $PSScriptRoot rev-parse --show-toplevel).Trim()
}
$RepoRoot = (Resolve-Path $RepoRoot).Path
if (-not $OutputDirectory) {
    $OutputDirectory = Join-Path $RepoRoot '.nuget-bump'
}
$cacheDirectory = Join-Path $OutputDirectory 'cache'
New-Item -ItemType Directory -Force -Path $cacheDirectory | Out-Null
$OutputDirectory = (Resolve-Path $OutputDirectory).Path
$cacheDirectory = (Resolve-Path $cacheDirectory).Path

function Get-RepoRelativePath([string]$Path) {
    [System.IO.Path]::GetRelativePath($RepoRoot, $Path).Replace('\', '/')
}

#endregion

#region Parse the generated NugetPackages.cs files

$frameworksByArm = @{}
foreach ($framework in @(Get-SupportedFrameworks) + @(Get-AnyFramework)) {
    $frameworksByArm["$($framework.Major).$($framework.Minor)"] = $framework
}

function Get-ArmFramework([int]$Major, [int]$Minor) {
    $framework = $frameworksByArm["$Major.$Minor"]
    if ($framework) { return $framework }
    [pscustomobject]@{ Tfm = "fw$Major.$Minor"; Name = $null; Major = $Major; Minor = $Minor; Order = [version]"$Major.$Minor" }
}

function Read-NugetPackagesFile([string]$Path) {
    $constants = @{}
    $registrations = [System.Collections.Generic.List[object]]::new()
    $current = $null
    $lastArm = $null
    $depth = 0
    $ignoreDepths = [System.Collections.Generic.Stack[int]]::new()
    $pendingIgnore = $false
    $lineNumber = 0

    foreach ($line in [System.IO.File]::ReadAllLines($Path)) {
        $lineNumber++
        $trimmed = $line.Trim()

        if ($trimmed -match '^public\s+const\s+string\s+(\w+)\s*=\s*"([^"]+)"\s*;') {
            $constants[$Matches[1]] = $Matches[2]
        }

        # A hand-written block: `//IntentIgnore` before a statement, or an ignore attribute before a member.
        if ($trimmed -match '^//\s*IntentIgnore\b' -or $trimmed -match '^\[(IntentIgnore\w*|IntentManaged\([^\]]*Mode\.Ignore)') {
            $pendingIgnore = $true
        }

        if ($trimmed -match 'NugetRegistry\.Register\(\s*(?:"([^"]+)"|(\w+))') {
            $packageName = if ($Matches[1]) { $Matches[1] } elseif ($constants.ContainsKey($Matches[2])) { $constants[$Matches[2]] } else { $Matches[2] -replace 'PackageName$', '' }
            $current = [pscustomobject]@{
                Package = $packageName
                Manual  = ($ignoreDepths.Count -gt 0 -or $pendingIgnore)
                Line    = $lineNumber
                Arms    = [System.Collections.Generic.List[object]]::new()
            }
            $registrations.Add($current)
            $lastArm = $null
        }
        elseif ($current -and $trimmed -match '^\(\s*>=\s*(\d+)\s*,\s*>=\s*(\d+)\s*\)\s*=>\s*new\s+PackageVersion\(\s*"([^"]+)"\s*(?:,\s*(?:locked:\s*)?(true|false)\s*)?\)') {
            $framework = Get-ArmFramework ([int]$Matches[1]) ([int]$Matches[2])
            $lastArm = [pscustomobject]@{
                Major         = $framework.Major
                Minor         = $framework.Minor
                Tfm           = $framework.Tfm
                FrameworkName = $framework.Name
                Version       = $Matches[3]
                Locked        = ($Matches[4] -eq 'true')
                Dependencies  = [System.Collections.Generic.List[object]]::new()
            }
            $current.Arms.Add($lastArm)
        }
        elseif ($lastArm -and $trimmed -match '^\.WithNugetDependency\(\s*"([^"]+)"\s*,\s*"([^"]*)"\s*\)') {
            $lastArm.Dependencies.Add([pscustomobject]@{ id = $Matches[1]; version = $Matches[2] })
        }

        # Track braces outside string literals and comments, to know when a hand-written block ends.
        $code = ($line -replace '"(?:[^"\\]|\\.)*"', '""') -replace '//.*$', ''
        foreach ($character in $code.ToCharArray()) {
            if ($character -eq '{') {
                $depth++
                if ($pendingIgnore) {
                    $ignoreDepths.Push($depth)
                    $pendingIgnore = $false
                }
            }
            elseif ($character -eq '}') {
                if ($ignoreDepths.Count -gt 0 -and $ignoreDepths.Peek() -eq $depth) { [void]$ignoreDepths.Pop() }
                $depth--
            }
        }
        # An ignored field (`[IntentIgnore] private readonly ...;`) has no block.
        if ($pendingIgnore -and $code.TrimEnd().EndsWith(';')) { $pendingIgnore = $false }
    }

    , $registrations.ToArray()
}

$modules = [System.Collections.Generic.List[object]]::new()
foreach ($directory in Get-ChildItem -Path (Join-Path $RepoRoot 'Modules') -Directory | Sort-Object Name) {
    $nugetPackagesPath = Join-Path $directory.FullName 'NugetPackages.cs'
    if (-not (Test-Path $nugetPackagesPath)) { continue }

    $imodspecFile = Get-ChildItem -Path $directory.FullName -Filter '*.imodspec' -File | Select-Object -First 1
    $moduleId = $directory.Name
    $moduleVersion = $null
    if ($imodspecFile) {
        [xml]$imodspec = Get-Content -Raw -Path $imodspecFile.FullName
        $moduleId = $imodspec.package.id
        $moduleVersion = $imodspec.package.version
    }

    $matchesFilter = $false
    foreach ($pattern in $ModuleFilter) {
        if ($directory.Name -like $pattern -or $moduleId -like $pattern) { $matchesFilter = $true }
    }
    if (-not $matchesFilter) { continue }

    $csprojVersion = $null
    $csprojFile = Get-ChildItem -Path $directory.FullName -Filter '*.csproj' -File | Select-Object -First 1
    if ($csprojFile) {
        [xml]$csproj = Get-Content -Raw -Path $csprojFile.FullName
        $csprojVersion = @($csproj.Project.PropertyGroup | ForEach-Object { $_.Version } | Where-Object { $_ })[0]
    }

    $modules.Add([pscustomobject]@{
            Id               = $moduleId
            Folder           = $directory.Name
            Path             = Get-RepoRelativePath $directory.FullName
            Imodspec         = if ($imodspecFile) { Get-RepoRelativePath $imodspecFile.FullName } else { $null }
            Version          = $moduleVersion
            CsprojVersion    = $csprojVersion
            HasReleaseNotes  = Test-Path (Join-Path $directory.FullName 'release-notes.md')
            NugetPackagesCs  = Get-RepoRelativePath $nugetPackagesPath
            Registrations    = Read-NugetPackagesFile $nugetPackagesPath
        })
}

if ($modules.Count -eq 0) {
    throw "No module matched -ModuleFilter '$($ModuleFilter -join "', '")' under $(Join-Path $RepoRoot 'Modules')."
}

#endregion

#region Look up nuget.org

$packageIds = @($modules | ForEach-Object { $_.Registrations } | ForEach-Object { $_.Package } | Sort-Object -Unique)
Write-Host "Scanned $($modules.Count) modules; looking up $($packageIds.Count) packages on nuget.org..."

$lookups = $packageIds | ForEach-Object -ThrottleLimit $ThrottleLimit -Parallel {
    . ([scriptblock]::Create($using:helperSource))
    $packageId = $_
    $cacheFile = Join-Path $using:cacheDirectory "$($packageId.ToLowerInvariant()).json"
    if ($using:CacheMaxAgeHours -gt 0 -and (Test-Path $cacheFile) -and
        ((Get-Date) - (Get-Item $cacheFile).LastWriteTime).TotalHours -lt $using:CacheMaxAgeHours) {
        return Get-Content -Raw -Path $cacheFile | ConvertFrom-Json
    }
    try {
        $result = Get-NuGetPackageLatest -PackageId $packageId
    }
    catch {
        $result = [pscustomobject]@{ id = $packageId; found = $false; error = $_.Exception.Message }
    }
    if ($result.found) {
        $result | ConvertTo-Json -Depth 10 | Set-Content -Path $cacheFile -Encoding utf8
    }
    $result
}

$lookupById = @{}
foreach ($lookup in $lookups) { $lookupById[$lookup.id] = $lookup }

#endregion

#region Compare

$deltaRank = @{ none = 0; downgrade = 0; patch = 1; new = 1; minor = 2; major = 3 }

function Get-DeltaClass([string]$From, [string]$To) {
    if (-not $From) { return 'new' }
    $comparison = Compare-NuGetVersion $From $To
    if ($comparison -eq 0) { return 'none' }
    if ($comparison -gt 0) { return 'downgrade' }
    $a = ConvertTo-VersionParts $From
    $b = ConvertTo-VersionParts $To
    if ($a.Numbers[0] -ne $b.Numbers[0]) { return 'major' }
    if ($a.Numbers[1] -ne $b.Numbers[1]) { return 'minor' }
    return 'patch'
}

function Get-MaxDelta([string[]]$Deltas) {
    $max = 'none'
    foreach ($delta in $Deltas) {
        if ($deltaRank[$delta] -gt $deltaRank[$max]) { $max = $delta }
    }
    $max
}

#region Dependency compatibility with locked rows

# A locked row holds a package back on purpose, often to keep one framework on an older stack: the net6.0
# in-process Azure Functions rows hold Microsoft.Extensions.* on 6.x. Another package can still pull a locked
# package in transitively at a higher version, and the consumer's restore then fails with NU1605 (package
# downgrade). Microsoft.Azure.WebJobs.Extensions.Storage.Queues 5.3.10 is in the same major as the locked
# 5.0.0, but it requires Microsoft.Extensions.Hosting 10.0.10 and so Microsoft.Extensions.DependencyInjection
# 10.0.10, over the module's locked 6.0.2. So a locked line is only patched to a version whose dependency tree
# fits every other lock, and every row whose version would break a lock is reported.

$versionCacheDirectory = Join-Path $cacheDirectory 'versions'
New-Item -ItemType Directory -Force -Path $versionCacheDirectory | Out-Null
$versionIndexById = @{}
$requirementsMemo = @{}

# Every listed version of a package with its dependency groups: version -> (short framework -> dependencies).
# A package nuget.org cannot return is treated as having no versions, so it adds no requirements.
function Get-NuGetVersionIndex([string]$PackageId) {
    $key = $PackageId.ToLowerInvariant()
    if ($versionIndexById.ContainsKey($key)) { return $versionIndexById[$key] }
    $cacheFile = Join-Path $versionCacheDirectory "$key.json"
    if ($CacheMaxAgeHours -gt 0 -and (Test-Path $cacheFile) -and
        ((Get-Date) - (Get-Item $cacheFile).LastWriteTime).TotalHours -lt $CacheMaxAgeHours) {
        $index = Get-Content -Raw -Path $cacheFile | ConvertFrom-Json -AsHashtable
    }
    else {
        $index = @{}
        try {
            $registration = Invoke-NuGetRequest "https://api.nuget.org/v3/registration5-gz-semver2/$key/index.json"
            foreach ($page in $registration.items) {
                $items = if ($page.items) { $page.items } else { (Invoke-NuGetRequest $page.'@id').items }
                foreach ($item in $items) {
                    $entry = $item.catalogEntry
                    if ($entry.listed -eq $false) { continue }
                    $groups = @{}
                    foreach ($group in @($entry.dependencyGroups)) {
                        if ($null -eq $group) { continue }
                        $tfm = ConvertTo-ShortFramework $group.targetFramework
                        if ($groups.ContainsKey($tfm)) { continue }
                        $groups[$tfm] = @(@($group.dependencies) | Where-Object { $_ } | ForEach-Object {
                                @{ id = $_.id; version = (Get-RangeMinVersion $_.range) }
                            })
                    }
                    $index[$entry.version] = $groups
                }
            }
            ConvertTo-Json -InputObject $index -Depth 8 -Compress | Set-Content -Path $cacheFile -Encoding utf8
        }
        catch {
            Write-Warning "Could not read the versions of $PackageId from nuget.org: $($_.Exception.Message)"
        }
    }
    $versionIndexById[$key] = $index
    $index
}

# The dependency group NuGet takes for a consumer framework: that framework's own, else the nearest lower one
# it is compatible with (net8.0 takes net6.0, then netstandard2.1, then netstandard2.0), else "any".
function Select-DependencyGroup($Groups, [string]$Tfm) {
    if (-not $Groups -or $Groups.Count -eq 0) { return @() }
    $frameworks = @(Get-SupportedFrameworks) + @(Get-AnyFramework)
    $target = $frameworks | Where-Object Tfm -eq $Tfm | Select-Object -First 1
    foreach ($framework in $frameworks | Sort-Object Order -Descending) {
        if ($target -and $framework.Order -gt $target.Order) { continue }
        if ($Groups.ContainsKey($framework.Tfm)) { return @($Groups[$framework.Tfm]) }
    }
    @()
}

# NuGet resolves a dependency to the lowest listed version its range allows; a stable minimum never resolves
# to a prerelease.
function Resolve-MinimumVersion($Index, [string]$Minimum) {
    if ($Index.ContainsKey($Minimum)) { return $Minimum }
    $allowPrerelease = Test-PrereleaseVersion $Minimum
    $best = $null
    foreach ($version in $Index.Keys) {
        if (-not $allowPrerelease -and (Test-PrereleaseVersion $version)) { continue }
        if ((Compare-NuGetVersion $version $Minimum) -lt 0) { continue }
        if (-not $best -or (Compare-NuGetVersion $version $best) -lt 0) { $best = $version }
    }
    $best
}

# The highest minimum version at which each package is required anywhere in a package version's dependency
# tree, for a consumer framework, with the chain that requires it: lower-case id -> @{ id; version; via }.
function Get-TransitiveRequirements([string]$PackageId, [string]$Version, [string]$Tfm) {
    $memoKey = "$($PackageId.ToLowerInvariant())@$Version@$Tfm"
    if ($requirementsMemo.ContainsKey($memoKey)) { return $requirementsMemo[$memoKey] }

    $required = @{}
    $expanded = @{}
    $queue = [System.Collections.Generic.Queue[object]]::new()
    $queue.Enqueue(@{ id = $PackageId; version = $Version; via = "$PackageId $Version" })
    while ($queue.Count -gt 0) {
        $node = $queue.Dequeue()
        $index = Get-NuGetVersionIndex $node.id
        $resolved = Resolve-MinimumVersion $index $node.version
        if (-not $resolved) { continue }
        $nodeKey = "$($node.id.ToLowerInvariant())@$resolved"
        if ($expanded.ContainsKey($nodeKey)) { continue }
        $expanded[$nodeKey] = $true
        foreach ($dependency in Select-DependencyGroup $index[$resolved] $Tfm) {
            if (-not $dependency.version) { continue }
            $key = $dependency.id.ToLowerInvariant()
            $via = "$($node.via) → $($dependency.id) $($dependency.version)"
            if (-not $required.ContainsKey($key) -or (Compare-NuGetVersion $dependency.version $required[$key].version) -gt 0) {
                $required[$key] = @{ id = $dependency.id; version = $dependency.version; via = $via }
            }
            $queue.Enqueue(@{ id = $dependency.id; version = $dependency.version; via = $via })
        }
    }
    $requirementsMemo[$memoKey] = $required
    $required
}

# The first two segments of a package id, "Microsoft.Extensions" for Microsoft.Extensions.Logging.Abstractions.
function Get-PackageFamily([string]$PackageId) {
    ($PackageId.ToLowerInvariant().Split('.') | Select-Object -First 2) -join '.'
}

# What a module's locks hold on a consumer framework, leaving out one package's own rows:
#   Direct  the locked rows themselves. They are direct references in the consumer's project, so any higher
#           requirement on one is a downgrade, and restore fails with NU1605.
#   Tree    the packages in those rows' own dependency trees that ship in lockstep with them (the same id
#           family, the first two segments of the id, and the same major as the locked row), at the highest
#           version the trees need. A lock holds those to their major too:
#           the net6.0 in-process lock on Microsoft.Extensions.DependencyInjection 6.0.2 brings
#           DependencyInjection.Abstractions 6.0.0, and a package that needs Abstractions 8.x puts an 8.x
#           Microsoft.Extensions assembly under the in-process host, which restore does not catch. A dependency
#           on another major is only a floor (Microsoft.Azure.Functions.Extensions 1.1.0 needs
#           Microsoft.Extensions.* 2.1.0 or later) and holds nothing.
function Get-LockedBaseline($Module, [string]$ExceptPackage, $Framework) {
    $direct = @{}
    foreach ($registration in $Module.Registrations) {
        if ($registration.Manual -or $registration.Package -eq $ExceptPackage) { continue }
        $arm = Resolve-Arm $registration.Arms $Framework.Major $Framework.Minor
        if ($arm -and $arm.Locked) {
            $direct[$registration.Package.ToLowerInvariant()] = @{ id = $registration.Package; version = $arm.Version }
        }
    }
    $tree = @{}
    foreach ($key in $direct.Keys) {
        $lockedMajor = (ConvertTo-VersionParts $direct[$key].version).Numbers[0]
        $lockedFamily = Get-PackageFamily $direct[$key].id
        $required = Get-TransitiveRequirements $direct[$key].id $direct[$key].version $Framework.Tfm
        foreach ($dependencyKey in $required.Keys) {
            if ($direct.ContainsKey($dependencyKey)) { continue }
            $dependency = $required[$dependencyKey]
            if ((Get-PackageFamily $dependency.id) -ne $lockedFamily) { continue }
            if ((ConvertTo-VersionParts $dependency.version).Numbers[0] -ne $lockedMajor) { continue }
            if (-not $tree.ContainsKey($dependencyKey) -or (Compare-NuGetVersion $dependency.version $tree[$dependencyKey].version) -gt 0) {
                $tree[$dependencyKey] = @{ id = $dependency.id; version = $dependency.version; lockedBy = $direct[$key].id }
            }
        }
    }
    @{ Direct = $direct; Tree = $tree; IsEmpty = ($direct.Count -eq 0) }
}

function Find-LockConflicts([string]$PackageId, [string]$Version, $Framework, $Baseline) {
    if (-not $Version -or $Baseline.IsEmpty) { return @() }
    $required = Get-TransitiveRequirements $PackageId $Version $Framework.Tfm
    $direct = @(foreach ($key in $Baseline.Direct.Keys) {
            if (-not $required.ContainsKey($key)) { continue }
            if ((Compare-NuGetVersion $required[$key].version $Baseline.Direct[$key].version) -le 0) { continue }
            [pscustomobject]@{
                tfm             = $Framework.Tfm
                kind            = 'direct'
                lockedPackage   = $Baseline.Direct[$key].id
                lockedVersion   = $Baseline.Direct[$key].version
                lockedBy        = $Baseline.Direct[$key].id
                requiredVersion = $required[$key].version
                via             = $required[$key].via
            }
        })
    $tree = @(foreach ($key in $Baseline.Tree.Keys) {
            if (-not $required.ContainsKey($key)) { continue }
            $held = $Baseline.Tree[$key]
            if ((ConvertTo-VersionParts $required[$key].version).Numbers[0] -le (ConvertTo-VersionParts $held.version).Numbers[0]) { continue }
            [pscustomobject]@{
                tfm             = $Framework.Tfm
                kind            = 'tree'
                lockedPackage   = $held.id
                lockedVersion   = $held.version
                lockedBy        = $held.lockedBy
                requiredVersion = $required[$key].version
                via             = $required[$key].via
            }
        })
    @($direct) + @($tree)
}

# The consumer frameworks that take a given row of a registration.
function Get-ServedFrameworks($Registration, [string]$Tfm) {
    @($consumerFrameworks | Where-Object {
            $arm = Resolve-Arm $Registration.Arms $_.Major $_.Minor
            $arm -and $arm.Tfm -eq $Tfm
        })
}

#endregion

# The newest version in a locked row's major line that is newer than the row and fits the module's other
# locks on every consumer framework the row serves: keeping a lock does not have to mean missing its patches,
# but a patch must not pull another locked package past its lock. Newer versions in the line that would are
# returned as `skipped`, with the conflicts of the newest. Its dependencies are those NuGet would pick for the
# row's framework.
function Get-LatestInLine($Module, [string]$PackageId, [string]$Version, [string]$Tfm) {
    $index = Get-NuGetVersionIndex $PackageId
    $major = (ConvertTo-VersionParts $Version).Numbers[0]
    $allowPrerelease = Test-PrereleaseVersion $Version
    [string[]]$candidates = @($index.Keys | Where-Object {
            (ConvertTo-VersionParts $_).Numbers[0] -eq $major -and
            ($allowPrerelease -or -not (Test-PrereleaseVersion $_)) -and
            (Compare-NuGetVersion $_ $Version) -gt 0
        })
    if ($candidates.Count -eq 0) { return $null }
    # Newest first.
    [array]::Sort($candidates, [System.Comparison[string]] { param($a, $b) Compare-NuGetVersion $b $a })

    $registration = $Module.Registrations | Where-Object { $_.Package -eq $PackageId -and -not $_.Manual } | Select-Object -First 1
    $served = if ($registration) { @(Get-ServedFrameworks $registration $Tfm) } else { @() }

    $chosen = $null
    $skipped = [System.Collections.Generic.List[string]]::new()
    $skippedConflicts = @()
    foreach ($candidate in $candidates) {
        $conflicts = @(foreach ($framework in $served) {
                Find-LockConflicts $PackageId $candidate $framework (Get-LockedBaseline $Module $PackageId $framework)
            })
        if ($conflicts.Count -eq 0) {
            $chosen = $candidate
            break
        }
        if ($skipped.Count -eq 0) { $skippedConflicts = $conflicts }
        $skipped.Add($candidate)
    }

    $dependencies = if ($chosen) {
        @(Select-DependencyGroup $index[$chosen] $Tfm | ForEach-Object { [pscustomobject]@{ id = $_.id; version = $_.version } })
    }
    else { @() }
    [pscustomobject]@{
        version          = $chosen
        dependencies     = $dependencies
        skipped          = $skipped.ToArray()
        skippedConflicts = $skippedConflicts
    }
}

# Minor and major deltas, and any delta on a 0.x package, get their release notes read.
function Test-NeedsBreakingReview([string]$From, [string]$To, [string]$Delta) {
    if ($Delta -in 'minor', 'major') { return $true }
    if ($Delta -eq 'patch' -and $From -and (ConvertTo-VersionParts $From).Numbers[0] -eq 0) { return $true }
    $false
}

# The row a consumer gets: the generated switch takes the first arm, in order, that the framework satisfies.
function Resolve-Arm($Arms, [int]$Major, [int]$Minor) {
    foreach ($arm in $Arms) {
        if ($Major -ge $arm.Major -and $Minor -ge $arm.Minor) { return $arm }
    }
    $null
}

function Format-Changes($Changes, [switch]$WithPackage) {
    $groups = @($Changes | Group-Object { "$($_.before)|$($_.after)" })
    $parts = foreach ($group in $groups) {
        $first = $group.Group[0]
        $text = if ($first.before) { "$($first.before) → $($first.after)" } else { "added $($first.after)" }
        if ($groups.Count -gt 1 -or -not $first.before) {
            $text += " ($((@($group.Group | ForEach-Object tfm | Sort-Object -Unique { [version]($_ -replace '^net(standard)?', '') } -Descending)) -join ', '))"
        }
        $text
    }
    $parts -join '; '
}

$consumerFrameworks = @(Get-SupportedFrameworks | Where-Object { $_.Tfm -like 'net[0-9]*' } | Sort-Object Order -Descending)

$usagesByPackage = @{}
$errors = [System.Collections.Generic.List[object]]::new()

foreach ($module in $modules) {
    $module | Add-Member -NotePropertyName Usages -NotePropertyValue ([System.Collections.Generic.List[object]]::new())
    foreach ($registration in $module.Registrations) {
        $lookup = $lookupById[$registration.Package]
        if (-not $lookup -or -not $lookup.found) { continue }

        $latestByTfm = @{}
        foreach ($latest in @($lookup.latest)) { $latestByTfm[$latest.tfm] = $latest }

        $rows = [System.Collections.Generic.List[object]]::new()
        foreach ($arm in $registration.Arms) {
            $latest = $latestByTfm[$arm.Tfm]
            if (-not $latest) {
                $action = if ($arm.Locked) { 'locked-orphan' } else { 'remove' }
                $rows.Add([pscustomobject]@{ tfm = $arm.Tfm; frameworkName = $arm.FrameworkName; current = $arm.Version; latest = $null; locked = $arm.Locked; action = $action; delta = $null; dependencies = $null })
                continue
            }
            $comparison = Compare-NuGetVersion $arm.Version $latest.version
            $action = if ($comparison -eq 0) { 'current' } elseif ($comparison -gt 0) { 'ahead' } elseif ($arm.Locked) { 'locked-behind' } else { 'update' }
            $rows.Add([pscustomobject]@{
                    tfm           = $arm.Tfm
                    frameworkName = $arm.FrameworkName
                    current       = $arm.Version
                    latest        = $latest.version
                    locked        = $arm.Locked
                    action        = $action
                    delta         = Get-DeltaClass $arm.Version $latest.version
                    dependencies  = if ($action -eq 'update') { @($latest.dependencies) } else { $null }
                })
        }
        foreach ($latest in @($lookup.latest)) {
            if ($registration.Arms | Where-Object Tfm -eq $latest.tfm) { continue }
            # A new row that consumers on its framework would take instead of a locked row bypasses that lock,
            # e.g. unlocked net8.0+ rows above MediatR's locked netstandard2.0 row. It goes to the lock review.
            $framework = @(Get-SupportedFrameworks) + @(Get-AnyFramework) | Where-Object Tfm -eq $latest.tfm | Select-Object -First 1
            $shadowed = Resolve-Arm $registration.Arms $framework.Major $framework.Minor
            if ($shadowed -and $shadowed.Locked) {
                $rows.Add([pscustomobject]@{ tfm = $latest.tfm; frameworkName = $latest.frameworkName; current = $shadowed.Version; latest = $latest.version; locked = $true; action = 'locked-add'; delta = Get-DeltaClass $shadowed.Version $latest.version; dependencies = $null; shadowsTfm = $shadowed.Tfm })
                continue
            }
            $rows.Add([pscustomobject]@{ tfm = $latest.tfm; frameworkName = $latest.frameworkName; current = $null; latest = $latest.version; locked = $false; action = 'add'; delta = 'new'; dependencies = @($latest.dependencies) })
        }

        # What a consumer on each framework gets before and after the proposed row changes.
        $consumerChanges = [System.Collections.Generic.List[object]]::new()
        if (-not $registration.Manual) {
            $after = [System.Collections.Generic.List[object]]::new()
            foreach ($row in $rows) {
                if ($row.action -in 'remove', 'locked-add') { continue }
                $framework = if ($row.frameworkName) { @(Get-SupportedFrameworks) + @(Get-AnyFramework) | Where-Object Tfm -eq $row.tfm | Select-Object -First 1 } else { $null }
                $arm = $registration.Arms | Where-Object Tfm -eq $row.tfm | Select-Object -First 1
                $major = if ($arm) { $arm.Major } else { $framework.Major }
                $minor = if ($arm) { $arm.Minor } else { $framework.Minor }
                $version = if ($row.action -in 'update', 'add') { $row.latest } else { $row.current }
                $after.Add([pscustomobject]@{ Major = $major; Minor = $minor; Version = $version })
            }
            $afterArms = @($after | Sort-Object @{ Expression = 'Major'; Descending = $true }, @{ Expression = 'Minor'; Descending = $true })
            foreach ($framework in $consumerFrameworks) {
                $before = Resolve-Arm $registration.Arms $framework.Major $framework.Minor
                $afterArm = Resolve-Arm $afterArms $framework.Major $framework.Minor
                $beforeVersion = if ($before) { $before.Version } else { $null }
                $afterVersion = if ($afterArm) { $afterArm.Version } else { $null }
                if ($beforeVersion -eq $afterVersion -or -not $afterVersion) { continue }
                $consumerChanges.Add([pscustomobject]@{ tfm = $framework.Tfm; before = $beforeVersion; after = $afterVersion; delta = Get-DeltaClass $beforeVersion $afterVersion })
            }
        }

        # The version a consumer gets after the proposed changes, checked against the module's other locks on the
        # same framework. This also catches a row already in the model that breaks a lock.
        $lockConflicts = [System.Collections.Generic.List[object]]::new()
        if (-not $registration.Manual) {
            foreach ($framework in $consumerFrameworks) {
                $baseline = Get-LockedBaseline $module $registration.Package $framework
                if ($baseline.IsEmpty) { continue }
                $before = Resolve-Arm $registration.Arms $framework.Major $framework.Minor
                $afterArm = Resolve-Arm $afterArms $framework.Major $framework.Minor
                if (-not $afterArm) { continue }
                foreach ($conflict in Find-LockConflicts $registration.Package $afterArm.Version $framework $baseline) {
                    $conflict | Add-Member -NotePropertyName version -NotePropertyValue $afterArm.Version
                    $conflict | Add-Member -NotePropertyName proposed -NotePropertyValue (-not $before -or $before.Version -ne $afterArm.Version)
                    $lockConflicts.Add($conflict)
                }
            }
        }

        $usage = [pscustomobject]@{
            moduleId        = $module.Id
            moduleFolder    = $module.Folder
            manual          = $registration.Manual
            line            = $registration.Line
            rows            = $rows.ToArray()
            consumerChanges = $consumerChanges.ToArray()
            lockConflicts   = $lockConflicts.ToArray()
        }
        $module.Usages.Add([pscustomobject]@{ Package = $registration.Package; Usage = $usage })
        if (-not $usagesByPackage.ContainsKey($registration.Package)) {
            $usagesByPackage[$registration.Package] = [System.Collections.Generic.List[object]]::new()
        }
        $usagesByPackage[$registration.Package].Add($usage)
    }
}

foreach ($lookup in $lookups | Where-Object { -not $_.found }) {
    $usedBy = @($modules | Where-Object { $_.Registrations.Package -contains $lookup.id } | ForEach-Object Id)
    $errors.Add([pscustomobject]@{ package = $lookup.id; error = $lookup.error; modules = $usedBy })
}

$packageResults = [System.Collections.Generic.List[object]]::new()
foreach ($packageId in $packageIds) {
    $lookup = $lookupById[$packageId]
    if (-not $lookup.found) { continue }
    $usages = @($usagesByPackage[$packageId])
    $interesting = @($usages | ForEach-Object { $_.rows } | Where-Object { $_.action -ne 'current' })
    if ($interesting.Count -eq 0) { continue }

    $generated = @($usages | Where-Object { -not $_.manual })
    $changes = @($generated | ForEach-Object { $_.consumerChanges })
    $locked = @($generated | ForEach-Object { $_.rows } | Where-Object action -eq 'locked-behind')
    $delta = Get-MaxDelta @($changes | ForEach-Object delta)
    # Locked rows are researched in the lock review instead.
    $needsReview = [bool]@($changes | Where-Object { Test-NeedsBreakingReview $_.before $_.after $_.delta }).Count

    $packageResults.Add([pscustomobject]@{
            id                  = $packageId
            latestOverall       = $lookup.latestOverall
            usedPrerelease      = $lookup.usedPrerelease
            nugetUrl            = $lookup.nugetUrl
            projectUrl          = $lookup.projectUrl
            repositoryUrl       = $lookup.repositoryUrl
            releaseNotes        = $lookup.releaseNotes
            deprecation         = $lookup.deprecation
            deltaClass          = $delta
            needsBreakingReview = $needsReview
            change              = if ($changes.Count) { Format-Changes $changes } else { $null }
            latest              = $lookup.latest
            usages              = $usages
        })
}

$moduleResults = [System.Collections.Generic.List[object]]::new()
foreach ($module in $modules) {
    $generated = @($module.Usages | Where-Object { -not $_.Usage.manual })
    $changed = @($generated | Where-Object { @($_.Usage.rows | Where-Object action -in 'update', 'add', 'remove').Count -gt 0 })
    $lockedBehind = @($generated | ForEach-Object { $package = $_.Package; $_.Usage.rows | Where-Object action -in 'locked-behind', 'locked-add' | ForEach-Object {
                $line = if ($_.action -eq 'locked-behind') { Get-LatestInLine $module $package $_.current $_.tfm } else { $null }
                [pscustomobject]@{ package = $package; tfm = $_.tfm; frameworkName = $_.frameworkName; current = $_.current; latest = $_.latest; latestInLockedLine = $line.version; latestInLockedLineDependencies = $line.dependencies; lockedLineSkipped = $line.skipped; lockedLineSkippedConflicts = $line.skippedConflicts; delta = $_.delta; newRowShadowing = $_.shadowsTfm } } })
    $lockedOrphans = @($generated | ForEach-Object { $package = $_.Package; $_.Usage.rows | Where-Object action -eq 'locked-orphan' | ForEach-Object {
                $line = Get-LatestInLine $module $package $_.current $_.tfm
                [pscustomobject]@{ package = $package; tfm = $_.tfm; frameworkName = $_.frameworkName; current = $_.current; latestInLockedLine = $line.version; latestInLockedLineDependencies = $line.dependencies; lockedLineSkipped = $line.skipped; lockedLineSkippedConflicts = $line.skippedConflicts } } })
    $lockConflicts = @($generated | ForEach-Object { $package = $_.Package; $_.Usage.lockConflicts | ForEach-Object {
                [pscustomobject]@{ package = $package; version = $_.version; proposed = $_.proposed; tfm = $_.tfm; kind = $_.kind; lockedPackage = $_.lockedPackage; lockedVersion = $_.lockedVersion; lockedBy = $_.lockedBy; requiredVersion = $_.requiredVersion; via = $_.via } } })
    $manualBehind = @($module.Usages | Where-Object { $_.Usage.manual } | ForEach-Object { $package = $_.Package; $line = $_.Usage.line; $_.Usage.rows | Where-Object action -in 'update', 'locked-behind' | ForEach-Object {
                [pscustomobject]@{ package = $package; line = $line; tfm = $_.tfm; current = $_.current; latest = $_.latest; locked = $_.locked; delta = $_.delta } } })
    $ahead = @($module.Usages | ForEach-Object { $package = $_.Package; $_.Usage.rows | Where-Object action -eq 'ahead' | ForEach-Object {
                [pscustomobject]@{ package = $package; tfm = $_.tfm; current = $_.current; latest = $_.latest } } })

    if (-not ($changed.Count -or $lockedBehind.Count -or $lockedOrphans.Count -or $manualBehind.Count -or $ahead.Count -or $lockConflicts.Count)) { continue }

    $releaseNoteRows = @(foreach ($item in $changed) {
            $changes = @($item.Usage.consumerChanges)
            if ($changes.Count) { "``$($item.Package)`` $(Format-Changes $changes)" }
            else { "``$($item.Package)`` row changes only, no consumer-visible version change: review" }
        })
    $deltas = @($changed | ForEach-Object { $_.Usage.consumerChanges } | ForEach-Object delta)

    $moduleResults.Add([pscustomobject]@{
            id                  = $module.Id
            folder              = $module.Folder
            path                = $module.Path
            imodspec            = $module.Imodspec
            version             = $module.Version
            isPrerelease        = [bool]($module.Version -and (Test-PrereleaseVersion $module.Version))
            csprojVersion       = $module.CsprojVersion
            hasReleaseNotes     = $module.HasReleaseNotes
            nugetPackagesCs     = $module.NugetPackagesCs
            packagesToChange    = @($changed | ForEach-Object Package)
            deltaClass          = Get-MaxDelta $deltas
            needsBreakingReview = [bool]@($changed | ForEach-Object { $_.Usage.consumerChanges } | Where-Object { Test-NeedsBreakingReview $_.before $_.after $_.delta }).Count
            releaseNoteRows     = $releaseNoteRows
            lockedBehind        = $lockedBehind
            lockedOrphans       = $lockedOrphans
            lockConflicts       = $lockConflicts
            manualBehind        = $manualBehind
            ahead               = $ahead
        })
}

# The same package and framework at different versions across modules; the bump should converge them.
$drift = [System.Collections.Generic.List[object]]::new()
foreach ($packageId in $packageIds) {
    $byTfm = @{}
    foreach ($module in $modules) {
        foreach ($registration in $module.Registrations | Where-Object { $_.Package -eq $packageId -and -not $_.Manual }) {
            foreach ($arm in $registration.Arms) {
                if (-not $byTfm.ContainsKey($arm.Tfm)) { $byTfm[$arm.Tfm] = [System.Collections.Generic.List[object]]::new() }
                $byTfm[$arm.Tfm].Add([pscustomobject]@{ module = $module.Id; version = $arm.Version; locked = $arm.Locked })
            }
        }
    }
    foreach ($tfm in $byTfm.Keys | Sort-Object) {
        $versions = @($byTfm[$tfm] | Group-Object version)
        if ($versions.Count -le 1) { continue }
        $drift.Add([pscustomobject]@{
                package  = $packageId
                tfm      = $tfm
                versions = @($versions | ForEach-Object { [pscustomobject]@{ version = $_.Name; modules = @($_.Group | ForEach-Object { if ($_.locked) { "$($_.module) (locked)" } else { $_.module } }) } })
            })
    }
}

#endregion

#region Write outputs

$summary = [pscustomobject]@{
    modulesScanned          = $modules.Count
    packagesScanned         = $packageIds.Count
    packagesWithChanges     = @($packageResults | Where-Object change).Count
    packagesNeedingReview   = @($packageResults | Where-Object { $_.change -and $_.needsBreakingReview }).Count
    modulesToChange         = @($moduleResults | Where-Object { $_.packagesToChange.Count }).Count
    lockedRowsBehind        = @($moduleResults | ForEach-Object { $_.lockedBehind }).Count
    rowsBreakingALock       = @($moduleResults | ForEach-Object { $id = $_.id; $_.lockConflicts | ForEach-Object { "$id|$($_.package)|$($_.version)" } } | Sort-Object -Unique).Count
    manualRowsBehind        = @($moduleResults | ForEach-Object { $_.manualBehind }).Count
    lookupErrors            = $errors.Count
}

$candidates = [pscustomobject]@{
    schemaVersion = 1
    generatedAt   = (Get-Date).ToString('o')
    repoRoot      = $RepoRoot
    moduleFilter  = $ModuleFilter
    frameworks    = @(Get-SupportedFrameworks | ForEach-Object { [pscustomobject]@{ tfm = $_.Tfm; frameworkName = $_.Name } })
    summary       = $summary
    modules       = $moduleResults
    packages      = $packageResults
    drift         = $drift
    errors        = $errors
}
$candidatesPath = Join-Path $OutputDirectory 'candidates.json'
$candidates | ConvertTo-Json -Depth 12 | Set-Content -Path $candidatesPath -Encoding utf8

$md = [System.Text.StringBuilder]::new()
function Add-Line([string]$Text = '') { [void]$md.AppendLine($Text) }
function Format-Modules($Ids) { (@($Ids | Sort-Object -Unique) | ForEach-Object { "``$_``" }) -join ', ' }

Add-Line '<!-- nuget-bump:inventory:start -->'
Add-Line '## Inventory'
Add-Line
$filterText = ''
if (@($ModuleFilter | Where-Object { $_ -ne '*' }).Count) { $filterText = ' for `' + ($ModuleFilter -join ', ') + '`' }
Add-Line "Generated $((Get-Date).ToString('yyyy-MM-dd HH:mm')) by ``Get-NuGetBumpCandidates.ps1``$filterText. $($summary.modulesScanned) modules and $($summary.packagesScanned) packages scanned: $($summary.packagesWithChanges) packages move in $($summary.modulesToChange) modules, $($summary.packagesNeedingReview) need a breaking-change review, $($summary.lockedRowsBehind) locked rows and $($summary.manualRowsBehind) hand-written rows are behind, $($summary.rowsBreakingALock) rows would break another lock, and $($summary.lookupErrors) lookups failed."
Add-Line

Add-Line '### Packages to update'
Add-Line
$toUpdate = @($packageResults | Where-Object change | Sort-Object @{ Expression = { $deltaRank[$_.deltaClass] }; Descending = $true }, id)
if ($toUpdate.Count) {
    Add-Line '| Package | Change | Delta | Review | Modules |'
    Add-Line '| --- | --- | --- | --- | --- |'
    foreach ($package in $toUpdate) {
        $moduleIds = @($package.usages | Where-Object { -not $_.manual -and $_.consumerChanges.Count } | ForEach-Object moduleId)
        Add-Line "| [``$($package.id)``]($($package.nugetUrl)) | $($package.change) | $($package.deltaClass) | $(if ($package.needsBreakingReview) { 'yes' } else { 'no' }) | $(Format-Modules $moduleIds) |"
    }
}
else { Add-Line 'None.' }
Add-Line

Add-Line '### Locks with a newer version'
Add-Line
$lockRows = @($moduleResults | ForEach-Object { $module = $_; $_.lockedBehind | ForEach-Object { $_ | Add-Member -NotePropertyName module -NotePropertyValue $module.id -PassThru -Force } })
# "the locked 6.0.2", or "6.0.0 from the locked `Microsoft.Extensions.DependencyInjection`" for a tree conflict.
function Format-HeldBy($Conflict) {
    if ($Conflict.kind -eq 'tree') { return "$($Conflict.lockedVersion) from the locked ``$($Conflict.lockedBy)``" }
    "the locked $($Conflict.lockedVersion)"
}

function Format-SkippedInLine($Row) {
    $skipped = @($Row.lockedLineSkipped | Where-Object { $_ })
    if ($skipped.Count -eq 0) { return '' }
    $conflict = @($Row.lockedLineSkippedConflicts | Where-Object { $_ })[0]
    if (-not $conflict) { return "$($skipped -join ', ')" }
    $text = "$($skipped[0]) needs ``$($conflict.lockedPackage)`` ≥ $($conflict.requiredVersion) over $(Format-HeldBy $conflict) ($($conflict.tfm))"
    if ($skipped.Count -gt 1) { $text += "; $($skipped.Count - 1) more" }
    $text
}

if ($lockRows.Count) {
    Add-Line 'The latest in the locked line is the newest version in the row''s major whose dependencies fit the module''s other locks; newer ones that would not are listed as skipped.'
    Add-Line
    Add-Line '| Module | Package | Framework | Locked at | Latest in locked line | Skipped (breaks another lock) | Latest | Delta |'
    Add-Line '| --- | --- | --- | --- | --- | --- | --- | --- |'
    foreach ($row in $lockRows | Sort-Object package, module, tfm) {
        $framework = if ($row.newRowShadowing) { "$($row.tfm) (new row, would bypass the $($row.newRowShadowing) lock)" } else { $row.tfm }
        Add-Line "| ``$($row.module)`` | ``$($row.package)`` | $framework | $($row.current) | $($row.latestInLockedLine) | $(Format-SkippedInLine $row) | $($row.latest) | $($row.delta) |"
    }
}
else { Add-Line 'None.' }
Add-Line

Add-Line '### Rows that would break another lock'
Add-Line
$conflictRows = @($moduleResults | ForEach-Object { $module = $_; $_.lockConflicts | ForEach-Object { $_ | Add-Member -NotePropertyName module -NotePropertyValue $module.id -PassThru -Force } })
if ($conflictRows.Count) {
    Add-Line 'The version a consumer gets from the row, after the proposed changes, needs a package the module''s locks hold lower on the same framework. **direct**: the package is itself locked, so a consumer that references both fails restore with NU1605 (package downgrade). **tree**: the package is in a locked row''s own dependency tree and is needed at a higher major, which restore allows but the lock exists to prevent (an 8.x Microsoft.Extensions assembly under the net6.0 in-process host). Choose an older version for the row, or review the lock. The check assumes a consumer gets every locked row of the module on that framework; when the templates never emit the two packages into the same project, confirm it in the template code, record it and move on. "Already in the model" means the row needs no change from this run to break the lock.'
    Add-Line
    Add-Line '| Module | Package | Version | Frameworks | Kind | Needs | Over | Through |'
    Add-Line '| --- | --- | --- | --- | --- | --- | --- | --- |'
    $groups = $conflictRows | Group-Object { "$($_.module)|$($_.package)|$($_.version)|$($_.lockedPackage)|$($_.lockedVersion)|$($_.requiredVersion)" }
    foreach ($group in $groups | Sort-Object { $_.Group[0].module }, { $_.Group[0].package }, { $_.Group[0].kind }) {
        $first = $group.Group[0]
        $tfms = (@($group.Group | ForEach-Object tfm | Sort-Object -Unique { [version]($_ -replace '^net(standard)?', '') } -Descending)) -join ', '
        $version = if ($first.proposed) { $first.version } else { "$($first.version) (already in the model)" }
        Add-Line "| ``$($first.module)`` | ``$($first.package)`` | $version | $tfms | $($first.kind) | ``$($first.lockedPackage)`` ≥ $($first.requiredVersion) | $(Format-HeldBy $first) | $($first.via) |"
    }
}
else { Add-Line 'None.' }
Add-Line

$orphanRows = @($moduleResults | ForEach-Object { $module = $_; $_.lockedOrphans | ForEach-Object { $_ | Add-Member -NotePropertyName module -NotePropertyValue $module.id -PassThru -Force } })
if ($orphanRows.Count) {
    Add-Line '### Locked rows for a framework nuget.org no longer resolves'
    Add-Line
    Add-Line 'An unlocked row here would be deleted. Each lock still needs its reason reviewed.'
    Add-Line
    Add-Line '| Module | Package | Framework | Locked at | Latest in locked line | Skipped (breaks another lock) |'
    Add-Line '| --- | --- | --- | --- | --- | --- |'
    foreach ($row in $orphanRows | Sort-Object package, module, tfm) {
        Add-Line "| ``$($row.module)`` | ``$($row.package)`` | $($row.tfm) | $($row.current) | $($row.latestInLockedLine) | $(Format-SkippedInLine $row) |"
    }
    Add-Line
}

Add-Line '### Hand-written branches with a newer version'
Add-Line
$manualRows = @($moduleResults | ForEach-Object { $module = $_; $_.manualBehind | ForEach-Object { $_ | Add-Member -NotePropertyName module -NotePropertyValue $module.id -PassThru -Force } })
if ($manualRows.Count) {
    Add-Line 'These sit in `IntentIgnore` code in `NugetPackages.cs`, not in the model. Never updated automatically.'
    Add-Line
    Add-Line '| Module | Package | Line | Framework | Current | Latest | Locked |'
    Add-Line '| --- | --- | --- | --- | --- | --- | --- |'
    foreach ($row in $manualRows | Sort-Object module, package, tfm) {
        Add-Line "| ``$($row.module)`` | ``$($row.package)`` | $($row.line) | $($row.tfm) | $($row.current) | $($row.latest) | $(if ($row.locked) { 'yes' } else { 'no' }) |"
    }
}
else { Add-Line 'None.' }
Add-Line

$tfmRows = @($packageResults | ForEach-Object { $package = $_; $_.usages | Where-Object { -not $_.manual } | ForEach-Object { $usage = $_; $_.rows | Where-Object action -in 'add', 'remove' | ForEach-Object {
                [pscustomobject]@{ module = $usage.moduleId; package = $package.id; tfm = $_.tfm; action = $_.action; version = if ($_.action -eq 'add') { $_.latest } else { $_.current } } } } })
if ($tfmRows.Count) {
    Add-Line '### Framework rows to add or remove'
    Add-Line
    Add-Line '| Module | Package | Framework | Action | Version |'
    Add-Line '| --- | --- | --- | --- | --- |'
    foreach ($row in $tfmRows | Sort-Object module, package, tfm) {
        Add-Line "| ``$($row.module)`` | ``$($row.package)`` | $($row.tfm) | $($row.action) | $($row.version) |"
    }
    Add-Line
}

$aheadRows = @($moduleResults | ForEach-Object { $module = $_; $_.ahead | ForEach-Object { $_ | Add-Member -NotePropertyName module -NotePropertyValue $module.id -PassThru -Force } })
if ($aheadRows.Count) {
    Add-Line '### Ahead of nuget.org'
    Add-Line
    Add-Line 'Never downgraded. Usually a deliberate prerelease, or a version nuget.org has since unlisted.'
    Add-Line
    Add-Line '| Module | Package | Framework | Current | nuget.org |'
    Add-Line '| --- | --- | --- | --- | --- |'
    foreach ($row in $aheadRows | Sort-Object module, package, tfm) {
        Add-Line "| ``$($row.module)`` | ``$($row.package)`` | $($row.tfm) | $($row.current) | $($row.latest) |"
    }
    Add-Line
}

if ($drift.Count) {
    Add-Line '### Version drift across modules'
    Add-Line
    Add-Line 'The same package and framework at different versions in different modules, before this bump.'
    Add-Line
    Add-Line '| Package | Framework | Versions |'
    Add-Line '| --- | --- | --- |'
    foreach ($row in $drift) {
        $versions = ($row.versions | ForEach-Object { "$($_.version): $($_.modules -join ', ')" }) -join '<br>'
        Add-Line "| ``$($row.package)`` | $($row.tfm) | $versions |"
    }
    Add-Line
}

$deprecated = @($packageResults | Where-Object deprecation)
if ($deprecated.Count) {
    Add-Line '### Deprecated on nuget.org'
    Add-Line
    Add-Line '| Package | Latest | Reasons | Replacement | Message |'
    Add-Line '| --- | --- | --- | --- | --- |'
    foreach ($package in $deprecated) {
        $message = "$($package.deprecation.message)" -replace '\s+', ' '
        $replacement = ''
        if ($package.deprecation.alternatePackage) { $replacement = '`' + $package.deprecation.alternatePackage + '`' }
        Add-Line "| ``$($package.id)`` | $($package.latestOverall) | $($package.deprecation.reasons -join ', ') | $replacement | $message |"
    }
    Add-Line
}

if ($errors.Count) {
    Add-Line '### Lookup errors'
    Add-Line
    Add-Line '| Package | Error | Modules |'
    Add-Line '| --- | --- | --- |'
    foreach ($row in $errors) {
        Add-Line "| ``$($row.package)`` | $($row.error) | $(Format-Modules $row.modules) |"
    }
    Add-Line
}
Add-Line '<!-- nuget-bump:inventory:end -->'

$inventory = $md.ToString().TrimEnd()
$reportPath = Join-Path $OutputDirectory 'report.md'
if (Test-Path $reportPath) {
    $existing = Get-Content -Raw -Path $reportPath
    $pattern = '(?s)<!-- nuget-bump:inventory:start -->.*?<!-- nuget-bump:inventory:end -->'
    $report = if ($existing -match $pattern) { [regex]::Replace($existing, $pattern, { param($m) $inventory }) } else { "$inventory`n`n$existing" }
}
else {
    $report = @"
# NuGet package bump — $((Get-Date).ToString('yyyy-MM-dd'))

## Progress

- [x] 2. Inventory
- [ ] 3. Lock review
- [ ] 4. Breaking-change review
- [ ] 5. Model updated
- [ ] 6. Versions
- [ ] 7. Release notes
- [ ] 8. Regenerated and built
- [ ] 9. Tests updated and built
- [ ] 10. Report final

$inventory

## Lock review

## Breaking-change review

## Module versions

## Deferred
"@
}
Set-Content -Path $reportPath -Value $report.TrimEnd() -Encoding utf8

#endregion

Write-Host "Packages to update: $($summary.packagesWithChanges) ($($summary.packagesNeedingReview) need a breaking-change review) across $($summary.modulesToChange) modules."
Write-Host "Locked rows behind: $($summary.lockedRowsBehind). Hand-written rows behind: $($summary.manualRowsBehind). Lookup errors: $($summary.lookupErrors)."
Write-Host "Wrote $(Get-RepoRelativePath $candidatesPath) and $(Get-RepoRelativePath $reportPath)."
