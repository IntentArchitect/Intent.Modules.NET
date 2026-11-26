### Version 1.1.1

- Improvement: The `mediator-domain-event-handler` skill is shorter: repeated sections are gone and each rule is stated once. It keeps the rules on scope (change only what the task needs), domain language and following similar handlers. It also still tells the agent to read the handler, its events and the domain types it touches before writing, and before finishing to check that every modelled association is implemented and review its code against every rule.

### Version 1.1.0

- Fixed: The handler skill told agents to search for a `// IntentInitialGen` marker that generated stubs never contain; it now looks for bodies that still throw `NotImplementedException`.

### Version 1.0.0

- New Feature: Initial release. Domain events dispatched through Mediator: every modelled handler gets a stub, and your AI agent writes how it reacts.
