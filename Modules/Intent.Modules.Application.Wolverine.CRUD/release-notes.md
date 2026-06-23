### Version 1.0.0-pre.0

- Initial release.
- Automatically implements CRUD handler bodies for Wolverine CQRS command and query handlers using the Domain Interactions pattern (repository lookups, AutoMapper projections, create/update/delete operations).
- New Feature: Convention-based "get all" query handlers. A query that returns a collection of a DTO mapped from a domain entity, with no `Query Entity Action` association, now generates a `FindAllAsync` plus AutoMapper projection body — matching the `Intent.Application.MediatR.CRUD` convention path.
