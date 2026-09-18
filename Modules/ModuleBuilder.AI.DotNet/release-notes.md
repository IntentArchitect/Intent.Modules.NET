### Version 1.0.0

- Initial release: ships the `intent-dotnet-module-integration` AI agent skill into the consuming repo's `.claude/skills` and `.opencode/skills` trees, covering how to integrate with ASP.NET Core Controllers' (`Intent.AspNetCore.Controllers`) generated output, including a "Version compatibility" note flagging which facts are gated to a specific version range of the target module.
- Fixed: the ASP.NET Core Controllers reference file now documents that `GetReturnStatement` hardcodes the local variable name `result` — a real bug (a hand-written `return Ok(result);` copy that happened to already use that name) surfaced this gap when an integrator had to decompile the installed assembly to confirm it.
