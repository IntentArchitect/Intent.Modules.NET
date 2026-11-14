# Regenerates the C# Java lexer and parser in Generated\ from JavaLexer.g4 and JavaParser.g4 (grammars-v4 java/java at
# 85e2dd1779, with the classType patch). The generated files are checked in, so building and running the engine needs no
# Java; only this script does: a Java runtime (11 or later) and the ANTLR tool jar whose version matches the
# Antlr4.Runtime.Standard package the Stacks project references (4.13.1), from Maven Central
# (https://repo1.maven.org/maven2/org/antlr/antlr4/4.13.1/antlr4-4.13.1-complete.jar).
param(
    [Parameter(Mandatory)] [string]$Java,
    [Parameter(Mandatory)] [string]$AntlrJar
)
$ErrorActionPreference = 'Stop'
$out = "$PSScriptRoot\Generated"
if (Test-Path $out) { Remove-Item -Recurse -Force $out }
New-Item -ItemType Directory -Force $out | Out-Null

& $Java -jar $AntlrJar `
    -Dlanguage=CSharp `
    -package Intent.Modules.Common.DriftEngine.Stacks.Java.Grammar `
    -visitor -listener `
    -Xexact-output-dir -o $out -lib $out `
    "$PSScriptRoot\JavaLexer.g4" "$PSScriptRoot\JavaParser.g4"
if ($LASTEXITCODE -ne 0) { throw "The ANTLR tool failed ($LASTEXITCODE)" }

# The tool's side files aren't used at run time.
Get-ChildItem $out -Include *.interp, *.tokens -Recurse | Remove-Item
Get-ChildItem $out | Select-Object Name, Length
