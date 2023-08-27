# Intent.Dapr.AspNetCore.Bindings.Cron

This module allows execution of `Command`s by [Dapr on a CRON schedule](https://docs.dapr.io/reference/components-reference/supported-bindings/cron/).

## Enabling a CRON schedule for a `Command`

Right-click any command and apply the `Dapr Cron Binding` stereotype. If the `Command` was not already exposed over HTTP, it is configured to be so going forward.

Update the `Schedule` property to the desired schedule using the [Schedule Format as per Dapr docs](https://docs.dapr.io/reference/components-reference/supported-bindings/cron/#schedule-format).

## Constraints

When the `Dapr Cron Binding` stereotype is applied, `Command`s must adhere to the following constraints for the binding to be able to work:

- The `Http Settings`' `Verb` must be set to `POST`.
- The `Http Settings`' `Return Type Mediatype` must be set to `Default`.
- It cannot have any fields.
- It cannot have a return type.
