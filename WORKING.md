# WORKING.md — Wolverine CRUD Integration

## Current Project Goal
Extend the Wolverine CQRS ecosystem by introducing a new module `Intent.Modules.Application.Wolverine.CRUD` that automatically implements CRUD handler bodies using repository pattern and domain interactions mapping.

## Active Tasks
- [x] Scaffold the new `Intent.Modules.Application.Wolverine.CRUD` module project.
- [x] Implement `CqrsHandlerCrudExtension` inside the new module (domain interactions path, `command`/`query` parameter names).
- [x] Register `CqrsHandlerCrudExtension` in the Intent Architect Module Builder designer so it appears in the imodspec.
- [x] Install the new module in the `Wolverine.CQRS.TestApplication` test application (via modules.config).
- [x] Run the Software Factory on `Wolverine.CQRS.TestApplication` — 1 change applied (GetItemByIdQueryHandler got IMapper injection).
- [x] Verify compilation: Build succeeded — 0 errors.
- [ ] Verify runtime behavior of the test application (start app, hit endpoints).

## Completed Gated Tasks
- [x] Implement constructor-based command/query models for Wolverine CQRS templates.
- [x] Update controller dispatcher mapper to map controller parameters to constructor arguments (falling back to object initializers).
- [x] Verify compilation in `Wolverine.CQRS.TestApplication` for the constructor mapping.
