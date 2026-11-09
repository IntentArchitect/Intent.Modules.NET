# Intent.Hangfire — Context

## Module Interactions

- **Dispatcher for command-publishing jobs (MediatR or Mediator).** A Hangfire Job linked to a command looks up the command through the `Application.Contract.Command` role, which both `Intent.Application.MediatR` and `Intent.Application.Mediator` (from `1.0.1-pre.0`) fill. The template picks the `ISender` from the template that answered: `Intent.Application.Mediator.CommandModels` gives `Mediator.ISender`, anything else `MediatR.ISender`. The id is a string, with no project reference to the Mediator module, the same seam `Intent.QuartzScheduler` uses (1.0.17). Renaming or moving that template in Mediator's Module Builder breaks this silently. Added in `1.0.14-pre.0` (2026-11-09); before that the job always emitted `using MediatR` and did not compile under Mediator. `Intent.Application.Mediator` detects this module and installs `1.0.14-pre.0` or later.
- **The lookup runs in `OnBuild`, not the constructor.** Before 1.0.14 the method lambda called `GetTypeName` on the role during construction, which depends on template registration order and fails outright when no template fills the role. Lookups belong in build callbacks.
- **Tested in** `Tests/TestMediator.Hangfire` (Mediator) and `Tests/Hangfire.Tests` (MediatR).
