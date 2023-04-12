Get-ChildItem Tests/**/*.sln -Recurse -Depth 2 | % { dotnet build --no-incremental $_ }

Pause