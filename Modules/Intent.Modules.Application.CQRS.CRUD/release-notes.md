### Version 1.0.0

- New Feature: Initial release. Discovers Command and Query handlers by role (`TemplateRoles.Application.Handler.Command` / `.Query`) instead of a concrete transport type, so any transport module (Wolverine, MediatR, etc.) can supply the handler templates.
- New Feature: Implements the modelled Domain Interactions (`Create Entity Action`, `Update Entity Action`, `Query Entity Action`, etc.) inside the discovered handler's `Handle` method for both commands and queries.
- New Feature: Adds a convention-based "get all" fallback for queries with no modelled Domain Interactions that return a collection of a DTO mapped from a domain entity, generating a repository `FindAllAsync` call plus an AutoMapper projection.
