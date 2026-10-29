### Version 1.1.0

- Improvement: Added integration guides for the ASP.NET Core host (`Intent.AspNetCore`), Entity Framework Core (`Intent.EntityFrameworkCore`) and the repository interfaces (`Intent.Entities.Repositories.Api`), alongside the existing ASP.NET Core Controllers guide.
- Improvement: The skill no longer assumes every module works like Controllers. Each guide now states which dependency to take and how that module's generated code links back to the designer model.

### Version 1.0.0

- Initial release: ships the `intent-dotnet-module-integration` AI agent skill into the consuming repo's `.claude/skills` and `.opencode/skills` trees, covering how to integrate with ASP.NET Core Controllers' (`Intent.AspNetCore.Controllers`) generated output, including a "Version compatibility" note flagging which facts are gated to a specific version range of the target module.
