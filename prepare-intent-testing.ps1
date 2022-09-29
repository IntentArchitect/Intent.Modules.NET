$modulesFolderPath = "Intent.Modules"
$pathToModules = "Modules"
$solutionFile = "Intent.Modules.NET.Tests.isln"

$repoConfigContent = 
"<?xml version=""1.0"" encoding=""utf-8""?>
<assetRepositories>
  <entries>
    <entry>
      <name>CI Compiled Modules</name>
      <address>$($modulesFolderPath)</address>
      <isBuiltIn>false</isBuiltIn>
      <order>3</order>
    </entry>
  </entries>
</assetRepositories>"

$moduleLookup = @{}
$moduleFileNames = Get-ChildItem "./$($pathToModules)/$($modulesFolderPath)/*.imod" | % {
    $file = [System.IO.Path]::GetFileNameWithoutExtension($_.Name)
    $dotNumber = 0
    $dotIndex = -1
    $dotIndexNo3 = -1
    for ($i = $file.Length - 1; $i -gt 0; $i--) {
        $cur = $file[$i]
        if ($cur -eq '.') {
            $dotNumber++
            $dotIndex = $i
        }
        if (($dotNumber -eq 3) -and ([System.Char]::IsDigit($file[$i + 1]))) {
            $dotIndexNo3 = $dotIndex
        }
        if ($dotNumber -eq 4) {
            if ([System.Char]::IsDigit($file[$i + 1])) {
                $dotIndex = $i
            } else {
                $dotIndex = $dotIndexNo3
            }
            break
        }
    }

    if ($dotIndex -gt -1) {
        $moduleLookup.Add($file.Substring(0, $dotIndex), $file.Substring($dotIndex + 1))
    }
}

$repoConfigContent | Set-Content ./$pathToModules/intent.repositories.config -Encoding UTF8

$testSln = [xml] (Get-Content ./$pathToModules/$solutionFile -Encoding UTF8)

$testSln.solution.applications.application | % {
    $curLocation = Get-Location;
    $appRelPath = [System.IO.Path]::Combine($curLocation, $pathToModules, $_.relativePath)
    $basePath = [System.IO.Path]::GetDirectoryName($appRelPath)
    $modulesConfig = [System.IO.Path]::Combine($basePath, "modules.config")

    $modulesConfigContent = [xml] (Get-Content $modulesConfig -Encoding UTF8)
    $changed = $false
    $modulesConfigContent.modules.module | % { 
        $module = $_
        $moduleVersionFound = $moduleLookup[$module.moduleId]
        if ($moduleVersionFound -ne $null) { 
            $module.version = $moduleVersionFound
            $changed = $true
        }
    }
    if ($changed) {
        $modulesConfigContent.Save($modulesConfig)
    }
}