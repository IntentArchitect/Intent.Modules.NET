# Intent.Application.Interop.CRUD

This module installs the CRUD implementation module that matches the dispatcher your application uses. Install it once and you get CRUD handler and service implementations for MediatR, Wolverine or Traditional Services without choosing the dispatcher-specific CRUD module yourself.

It generates no files and has no settings. It contains only module interoperability rules.

## Which CRUD Module Gets Installed

When Intent Architect finds one of the dispatcher modules below in the application, it installs the matching CRUD module:

| Dispatcher module installed                 | CRUD module installed                                                         |
| ------------------------------------------- | ----------------------------------------------------------------------------- |
| `Intent.Application.MediatR`                | `Intent.Application.MediatR.CRUD` (7.2.2 or later)                            |
| `Intent.Application.Wolverine`              | `Intent.Application.CQRS.CRUD` (1.0.0 or later)                               |
| `Intent.Application.ServiceImplementations` | `Intent.Application.ServiceImplementations.Conventions.CRUD` (5.3.5 or later) |

If an application has a CQRS dispatcher (MediatR or Wolverine) and Traditional Services installed, both matching CRUD modules are installed.

The CRUD modules do the actual work: they implement the bodies of command handlers, query handlers and service operations from the modelled domain interactions. See each module's own documentation for what it generates.

## Mediator Is Not Covered

No rule targets `Intent.Application.Mediator`. With Mediator, an AI agent writes every handler body, and `Intent.Application.CQRS.CRUD` would overwrite those bodies, so no CRUD module is installed for Mediator's handlers. If the application also has Traditional Services installed, that rule still fires and installs `Intent.Application.ServiceImplementations.Conventions.CRUD` for the services.

## Removing CRUD

Interoperability only installs modules. It never removes them. Uninstalling this module leaves the CRUD module it installed in place.

To remove CRUD from an application, uninstall both this module and the CRUD module it installed. If you uninstall only the CRUD module, this module can install it again the next time modules are installed or updated.

## Related Modules

- Intent.Application.MediatR.CRUD
- Intent.Application.CQRS.CRUD
- Intent.Application.ServiceImplementations.Conventions.CRUD
