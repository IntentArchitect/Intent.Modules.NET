### Version 1.1.1

- Improvement: The `mediator-command-handler` and `mediator-query-handler` skills are shorter, keeping only the rules shown to change what an agent writes.

### Version 1.1.0

- Improvement: Scheduled Jobs from `Intent.QuartzScheduler` and Hangfire Jobs from `Intent.Hangfire` can publish commands.
- Fixed: The handler skills told agents to search for a `// IntentInitialGen` marker that generated stubs never contain; they now look for bodies that still throw `NotImplementedException`.

### Version 1.0.0

- New Feature: Initial release. CQRS on the source-generated Mediator library: Intent generates the requests, handlers and pipeline, and your AI agent writes the business logic.
