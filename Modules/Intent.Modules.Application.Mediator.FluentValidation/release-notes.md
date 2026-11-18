### Version 1.0.1

- Fixed: Command and query validators with nested DTO properties no longer generate a redundant null-forgiving operator (`!`) on `GetValidator<T>()` that SonarAnalyzer rule S8969 reports, which failed builds with warnings treated as errors.

### Version 1.0.0

- New Feature: Initial release. FluentValidation for Mediator: every command and query is validated before its handler runs, with rules your AI agent writes from your domain.
