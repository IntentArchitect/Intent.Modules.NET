# Intent.AspNetCore.OData.EntityFramework

This module adds OData Query support to your domain designers, specifically to `Entity`s.

## What is OData Query?

OData Query is a URL-based query language used to filter, sort, and retrieve specific information from resources exposed through OData APIs. These queries follow a specific syntax based on URL parameters. The language enables developers to perform operations like filtering, sorting, selecting specific fields, and navigating relationships between entities.

This functionality is implemented using the `Microsoft.AspNetCore.OData` NuGet package.

For more information on OData querying, refer to the official [documentation](https://learn.microsoft.com/en-us/odata/concepts/queryoptions-overview).

> This module works in conjunction with `Intent.EntityFrameworkCore`.

## Domain Designer

To leverage OData Query functionality, simply apply the `Expose As OData` stereotype to the relevant `Entity`s in the domain designer.

![OData EntityFramework Stereotype](images/exposeasodata-stereotype.png)

The stereotype must meet the following criteria:

- The `Class` must be part of a repository.

## Using the "Expose As OData" Options

By default, all options are enabled. Checking any of these options will add the corresponding `Controller Action Method` to each `{Entity}Controller`.

![OData EntityFramework Stereotype](images/exposeasodata-stereotype-options.png)

## What's in This Module?

This module consumes your `Expose As OData`-enabled `Entity`s, which you design in the Domain Designer, and generates the following:

- Container registrations for OData infrastructure, enabling all `OData` options within the container.
- Entity mapping, including any composite `Entity`s.
- A controller for each `Entity`.
- Action methods added to controllers based on the configured options.
- Swashbuckle/Swagger integration, updating the Swagger schema to include OData query parameter options.
- Compatible with [Intent.AspNetCore.ODataQuery](https://docs.intentarchitect.com/articles/modules-dotnet/intent-aspnetcore-odataquery/intent-aspnetcore-odata-entityframework.html).  
  > **Note:** All `ODataOptions` in your container registration will be enabled.
