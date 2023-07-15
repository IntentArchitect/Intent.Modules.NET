### Version 5.1.2

- Fixed: In some cases there are "traceId" data already specified when the Generic Exception handler executes, this will ensure that the code won't break when that is the case.

### Version 5.1.1

- Fixed: Minor fixes around Trace Id and Startup.cs code generation.

### Version 5.1.0

- New: Added `UseExceptionHandler` to catch unhandled exceptions and return a `ProblemDetails` response for an HTTP 500 status code to allow for the `Trace ID` to be returned in the response for using with Telemetry systems like Azure Application Insights in order to troubleshoot errors. To make use of the `Trace ID` examine this example `"00-7e7ba8aacdb26b4300226a11d2e3db91-fc881cc2439952cf-01"`. Copy the number between the first and second `-` as this will be your `Operation Id`: `7e7ba8aacdb26b4300226a11d2e3db91`.

### Version 5.0.0

- Updated Program template to also use the Builder Pattern.

### Version 4.0.2

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.0

- New: Upgraded Templates to use new Builder Pattern paradigm.