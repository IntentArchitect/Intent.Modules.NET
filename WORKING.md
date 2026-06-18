# WORKING.md — feature/wolverine

## Open SF Cycle (must close before any new template work)

**Commit:** `204ea7b921` — `refactor(wolverine): move middleware DI registration into WolverineConfiguration`

**What changed:** `WolverineConfigurationTemplatePartial.cs` and `WolverineRegistrationFactoryExtension.cs` were edited to move 6 `AddTransient<*Middleware>()` calls out of `AddInfrastructure()` and into `WolverineConfiguration.Configure()` via `opts.Services.AddTransient<T>()`.

**Cycle status:**
- [x] Step 1 — template edited
- [x] Step 2 — module built (0 errors)
- [x] Step 3 — module reinstalled
- [ ] Step 4 — SF on target app (failed with transient AutoMapper KeyNotFoundException; never retried)
- [ ] Step 5 — staged diff inspected
- [ ] Step 6 — staged changes applied
- [ ] Step 7 — target solution built

**What to do first this session:** retry `run_software_factory(41ee3259-07cd-468f-b00f-4cdcb26bec14)`, inspect diffs, apply, build. Only then proceed to roadmap work.

**Note:** The test app files (`WolverineConfiguration.cs`, `DependencyInjection.cs`) were manually updated to the expected output and committed alongside the module code. The SF run should produce 0 staged changes — that will confirm the template and the on-disk files are in sync.

---

## Next planned work (do not start until open cycle is closed)

See `plans/wolverine-roadmap/plan.mdx` for the full roadmap. Priority order from the plan:

1. Dandré creates three module shells (Azure Functions, Fast Endpoints, AWS Lambda dispatch)
2. Implement FluentValidation validator templates (`Intent.Application.Wolverine.FluentValidation`)
3. Implement DomainEvents service template (`Intent.Application.Wolverine.DomainEvents`)
4. Add outbox factory extensions to MassTransit and NServiceBus modules (depends on UoW middleware role being stable)
