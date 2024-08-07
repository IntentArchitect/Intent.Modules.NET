@echo off
setlocal
REM Get the directory of the batch file
set BATCH_DIR=%~dp0

REM Run the executable with the batch file's directory as a parameter
dotnet run --project "..\..\..\Intent.NuGetReferenceUpdater\Intent.NuGetReferenceUpdater\Intent.NuGetReferenceUpdater.csproj" -- --directory %BATCH_DIR% --force-code-updates true
endlocal


