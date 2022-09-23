### Version 3.3.7

- New: HttpClientRequestException added which contains its own response content in error scenarios.
- Update: Leveraging new internal interface for obtaining service proxy information.

### Version 3.3.6

- Update: Generates code from referenced services modeled in `Service Proxies` designer in the form of a Service Interface (that is fully `async`/`await` enabled) and DTO classes (including Commands and Queries).