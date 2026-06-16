# WORKING.md — Wolverine CRUD Integration

## Current Project Goal
Extend the Wolverine CQRS ecosystem by introducing a new module `Intent.Modules.Application.Wolverine.CRUD` that automatically implements CRUD handler bodies using repository pattern and domain interactions mapping.

## Active Tasks
- [ ] Scaffold the new `Intent.Modules.Application.Wolverine.CRUD` module project.
- [ ] Implement `CqrsHandlerCrudExtension` inside the new module.
- [ ] Implement CRUD mapping strategies for Wolverine handlers (handling `command`/`query` parameter names).
- [ ] Install the new module in the `Wolverine.CQRS.TestApplication` test application.
- [ ] Run the Software Factory on `Wolverine.CQRS.TestApplication` to generate CRUD handler bodies.
- [ ] Verify compilation and runtime behavior of the test application.

## Completed Gated Tasks
- [x] Implement constructor-based command/query models for Wolverine CQRS templates.
- [x] Update controller dispatcher mapper to map controller parameters to constructor arguments (falling back to object initializers).
- [x] Verify compilation in `Wolverine.CQRS.TestApplication` for the constructor mapping.
