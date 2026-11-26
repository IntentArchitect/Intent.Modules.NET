### Version 1.1.1

- Improvement: The `mediator-command-handler` and `mediator-query-handler` skills are shorter: repeated sections are gone and each rule is stated once. They keep the rules on scope (change only what the task needs), domain language, following similar handlers, checking existence and permission before a command changes state, and applying the same filters and visibility rules in queries. They also still tell the agent to read the handler, its request and the domain types it touches before writing, and before finishing to check that every modelled association is implemented and review its code against every rule.

### Version 1.1.0

- Improvement: Scheduled Jobs from `Intent.QuartzScheduler` and Hangfire Jobs from `Intent.Hangfire` can publish commands.
- Fixed: The handler skills told agents to search for a `// IntentInitialGen` marker that generated stubs never contain; they now look for bodies that still throw `NotImplementedException`.

### Version 1.0.0

- New Feature: Initial release. CQRS on the source-generated Mediator library: Intent generates the requests, handlers and pipeline, and your AI agent writes the business logic.
