Get-ChildItem Tests/**/*.sln -Recurse -Depth 2 | % { dotnet build $_ }

Pause