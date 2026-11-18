### Version 1.0.10

- Improvement: The Quartz job listener now matches the Quartz.NET version `Intent.QuartzScheduler` requests for the project. Where that is Quartz 4 (`Intent.QuartzScheduler` 1.2.0 on projects targeting only `.NET 10` or later), the listener uses `ValueTask` callbacks and `GroupMatcher` from the `Quartz` namespace, and is registered with an explicit matcher array so it compiles under any C# language version. Otherwise it is unchanged.

### Version 1.0.9

- Improvement: Updated NuGet package versions.

### Version 1.0.8

- Fixed: Minimum client version.

### Version 1.0.7

- Improvement: Documentation and ProjectUrl link added.

### Version 1.0.6

- Improvement: Updated NuGet package versions.

### Version 1.0.5

- Improvement: Updated NuGet package versions.

### Version 1.0.4

- Improvement: Updated module NuGet packages infrastructure.

### Version 1.0.3

- Improvement: Updated NuGet packages to latest stables.
- Fixed: JobListeners are set up as [singleton](https://stackoverflow.com/questions/67323533/how-can-i-use-di-in-quartz-joblistener) and adjusted accordingly and Scoped Service Providers are introduced in `BugSnagQuartzJobListener` for resolving BugSnag.

### Version 1.0.2

- Fixed: Missed using directive for JobListeners.

### Version 1.0.1

- Improvement: Logs unhandled exceptions for Quartz Scheduler.

### Version 1.0.0

- New Feature: Integrate with [Bugsnag](https://www.bugsnag.com/) which is a cloud-based error monitoring and reporting tool for web and mobile apps.
- Improvement: Also tracks your Activity / Trace ID along with the Unhandled Exception.
