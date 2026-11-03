# Intent.QuartzScheduler — Context

## Module Interactions

- **Dispatcher for command-publishing jobs (MediatR or Mediator).** A Scheduled Job linked to a command looks up the
  command through the `Application.Contract.Command` role, which both `Intent.Application.MediatR` and
  `Intent.Application.Mediator` (from `1.0.1-pre.0`) fill. The template picks the `ISender` from the template that
  answered: `Intent.Application.Mediator.CommandModels` gives `Mediator.ISender`, anything else `MediatR.ISender`.
  The id is a string, with no project reference to the Mediator module, which is the same seam pattern EF Core and the
  eventing modules use for Mediator. Renaming or moving that template in Mediator's Module Builder breaks this
  silently. Added in `1.0.17-pre.0` (2026-11-03) after a client report that a linked job could not generate when
  Mediator was installed.
- **The lookup runs in `OnBuild`, not the constructor.** Before 1.0.17 the constructor called `GetTypeName` on the role,
  and with no template filling it the Software Factory process exited with no error message. Lookups belong in build
  callbacks anyway, because constructors run while templates are still being registered.
- **Tested in** `Tests/TestMediator.Quartz` (Mediator) and `Tests/AdvancedMappingCrud.Cosmos.Tests` (MediatR,
  `CommandDelegateJob`).
