# Intent.ApiGateway.Ocelot

This module implements the Ocelot API Gateway in a ASP.NET Core application.

# What is Ocelot?

Ocelot is aimed at people using .NET running a microservices / service-oriented architecture that need a unified point of entry into their system.
-[Website](https://ocelot.readthedocs.io/en/latest/introduction/bigpicture.html#big-picture)

# Modeling API Gateway Routes

![Ocelot API Gateway modeling](images/ocelot-modeling.png)

The following steps are performed inside the Services designer:

- First you need to add a reference to a Services package containing all the downstream service endpoints.
  - Right-click on the Service package and select `Add Package Reference`.
  - Locate the Service package from another Intent Architect application that will act as the downstream service.
  - Click on DONE.
- Add downstream endpoints:
  - Right-click on the diagram background and select `Add to diagram`.
  - Select the downstream endpoints you would like to connect to. They would be in the form of `Command`/`Query`/`Service` endpoints.
  - Click on DONE.
- Add Api Gateway routes:
  - Right-click on the diagram background and add a new `Api Gateway Route` element.
  - Right-click on it and select `New Route Association`.
  - Select a downstream endpoint to connect to and click to snap it into place.

# Inspect the `ocelot.json` file

The routes will be generated inside the `ocelot.json` file:

```json
{
  "Routes": [
  {
      "Key": "CreateClientCommandGatewayRoute",
      "UpstreamPathTemplate": "/api/clients",
      "UpstreamHttpMethod": [
          "Post"
      ],
      "DownstreamPathTemplate": "/api/clients",
      "DownstreamHttpMethod": "POST",
      "DownstreamHostAndPorts": [
        {
           "Host": "{Clients.Host.Name}"
        }
      ]
  },
  // ...
```

Each route's `Key` is the name of its API Gateway Route, so you can refer to it from an Ocelot aggregate's `RouteKeys`. Route names must be unique.

## Editing `ocelot.json` by hand

You can add any Ocelot setting to a generated route, for example `AuthenticationOptions`, `RateLimitOptions` or `Priority`, and it is kept when the Software Factory runs again. Routes are matched on their `UpstreamPathTemplate` and `DownstreamHttpMethod`, so your additions stay with their route when other routes are added or removed. If you change either of those two values in the designer, the route counts as new and your additions to it are lost.

Removing a route from the designer removes it from `ocelot.json`. Top-level sections you add by hand, such as `GlobalConfiguration`, are left alone.

## Hosts

You will see that each route entry has a Host value corresponding to the package name being imported.
This is a substitution of the actual host name and port number.

Configure those services inside the `appsettings.json` file:

```json
{
  //...
  "Ocelot": {
    "Hosts": {
      "Clients.Host.Name": "https://localhost:44300"
    }
  }
}
```