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
