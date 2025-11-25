# Intent.Dapr.AspNetCore.ServiceInvocation

This module provides service invocation capabilities for Dapr.

## What is Dapr Service Invocation?

Dapr service invocation enables applications to communicate with each other through well-defined APIs using standard protocols such as HTTP or gRPC. Dapr discovers and resolves the location of services, and provides a consistent way to call them, whether they are running on a local machine, in the cloud, or on the edge.

For more information, see the [official Dapr documentation on service invocation](https://docs.dapr.io/developing-applications/building-blocks/service-invocation/service-invocation-overview/).

## What this module generates

This module generates:
* C# interfaces for services that can be invoked.
* Dapr client-side proxy implementations for those interfaces.

## How to model service invocations

To define HTTP service invocations that will use Dapr's service-to-service invocation, model your external service calls using Intent Architect's **Services Designer**. The generated proxies will automatically route calls through Dapr's sidecar.

For detailed guidance on modeling HTTP endpoint invocations, see the [Invoking HTTP Endpoints documentation](https://docs.intentarchitect.com/articles/application-development/modelling/services-designer/invoking-http-endpoints/invoking-http-endpoints.html).

## When to use this

Use this module when you want to build a microservices application where services need to communicate with each other directly and you are using Dapr to manage your services. Dapr handles service discovery, mutual TLS, retries, and observability for these calls.

## Related Modules

* **Intent.Dapr.AspNetCore.Pubsub**: For event-based communication between services.

## External Resources

* [Dapr Service Invocation building block](https://docs.dapr.io/developing-applications/building-blocks/service-invocation/service-invocation-overview/)
* [Intent Architect Invoking HTTP Endpoints](https://docs.intentarchitect.com/articles/application-development/modelling/services-designer/invoking-http-endpoints/invoking-http-endpoints.html)
