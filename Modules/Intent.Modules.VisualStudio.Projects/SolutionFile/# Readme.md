# Readme

This folder contains a copy of `SlnFile.cs` and any depenencies from https://github.com/dotnet/sdk/tree/156f1cbb2edd5eff2191a91762ca1b4f8de4d66c/src/Cli/Microsoft.DotNet.Cli.Sln.Internal.

The changes are as follows:

- All types have been made `internal`.
- `SlnFile` has been made `partial`.
- Additions to `SlnFile` have been applied in `SlnFile.Additions.cs`.
