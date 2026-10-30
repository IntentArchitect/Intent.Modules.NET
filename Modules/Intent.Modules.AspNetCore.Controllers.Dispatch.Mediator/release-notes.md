### Version 1.0.0

- New Feature: ASP.NET Core controllers dispatch to Mediator. Every Command and Query that has HTTP settings is grouped into a controller by its parent folder in the Services designer, each controller receives an `ISender`, and each action sends the request through it.
- Improvement: the module has its own icon, a teal hub-and-spoke badge shared by every Mediator module and distinct from MediatR's.
