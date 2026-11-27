# Context: Intent.FastEndpoints

## Purpose
Generates FastEndpoints endpoint classes from Services designer elements (commands, queries and service
operations exposed over HTTP).

## Architectural Decisions
- **A `GET`/`QUERY` with no return type is annotated 204, not 200 (since 1.3.1).** The runtime (`Utils.cs`)
  already returns `TypedResults.NoContent()`, but `AddDescriptionConfiguration` emitted
  `b.Produces(StatusCodes.Status200OK)`. `GET`/`QUERY` now default to 204 when `Model.ReturnType` is null, as
  `PUT`/`PATCH` already did. A user-set `Success Response Code` still wins, because the default is passed through
  `_responseMapper.GetResponseStatusCodeEnum`. `DELETE` was split out of the shared branch and is intentionally
  still 200; `POST` stays 201. The `defaultValue` variable is declared once above the verb switch: a `var` declared in
  one `case` is in scope for the whole switch block, so it cannot be redeclared in the `PUT`/`PATCH` case.
  Needed because the `Intent.Metadata.WebApi` validation requiring a return type on `GET`/`QUERY` was removed.
  Regression anchor: the `FastEndpointsTest` test model has a `GET` with no return type, which must emit
  `b.Produces(StatusCodes.Status204NoContent)`; its `DELETE` endpoints must keep `Status200OK`.
