### Version 1.0.2

- Fixed: `WolverineRegistrationFactoryExtension` threw `More than one instance of template App.Program was found` in applications with more than one ASP.NET Core host; Wolverine is now registered on every host's `Program` file.

### Version 1.0.1

- Improvement: Additional context for the AI around how to implement the handler for a Wolverine handler.

### Version 1.0.0

- New Feature: Initial release of the Wolverine application module.

> ⚠️ NOTE
> 
> When migrating from MediatR's patterns to Wolverine, any updates you made to the Pipeline Behaviours will need to be manually ported to the Middleware pipeline in Wolverine or it will be lost.
