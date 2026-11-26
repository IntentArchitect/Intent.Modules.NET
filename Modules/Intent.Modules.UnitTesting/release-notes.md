### Version 1.2.1

- Improvement: Changes how agents receive the unit test guidance. `unit-test-guidelines` is now a skill, loaded on demand from `.agents/skills/`, in place of the instruction that was always in context. Upgrading moves the skill's output and removes the old `.agents/instructions/unit-test-guidelines.md`. An application that targets several agent harnesses (for example `.claude`, `.codex` and `.github`) gets the skill in each harness's own skills folder. The text is shorter, keeps the Intent-specific rules, and no longer asks domain-service tests to verify `SaveChangesAsync`.

### Version 1.2.0

- Improvement: Updated NuGet package versions:
  - `coverlet.collector` 10.0.1 → 10.1.0
  - `Moq` 4.20.72 → 4.21.0
  - `NSubstitute` 5.3.0 → 6.2.0

### Version 1.1.2

- Improvement: Updated NuGet package versions.

### Version 1.1.1

- Improvement: Updated instruction format and generation to align with other modules.

### Version 1.1.0

- Improvement: Centralized AI task registration for generated unit tests into a factory extension instead of registering tasks inside individual templates.

### Version 1.0.5

- Improvement: Will now create the test project in the same solution folder as the application project.
- Fixed: Would cause software factory errors if being installed when the newer `Codebase Structure` designer was in use.

### Version 1.0.4

- Improvement: Updated NuGet package versions.

### Version 1.0.3

- Improvement: Updated NuGet package versions.

### Version 1.0.2

- Improvement: Updated module documentation to use centralized documentation site.
- Improvement: Installing `Tests` folder in VS Designer will be more intelligent with placement.
- Improvement: Additional support given for: Domain Event Handlers, Domain Services and Integration Event Handlers.

### Version 1.0.1

- Improvement: Updated NuGet package versions.

### Version 1.0.0

- Improvement: Updated NuGet package versions.
- Initial version.
