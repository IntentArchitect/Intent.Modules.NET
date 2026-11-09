### Version 1.0.1

- Fixed: The handler skill told agents to search for a `// IntentInitialGen` marker that generated stubs never contain; it now looks for bodies that still throw `NotImplementedException`.

### Version 1.0.0

- New Feature: Initial release. Domain events dispatched through Mediator: every modelled handler gets a stub, and your AI agent writes how it reacts.
