using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.OpenApi;

namespace HttpQuery.FastEndpoints.Api.Filters
{
    /// <summary>
    /// Adds HTTP QUERY operations to the Microsoft.AspNetCore.OpenApi document.
    /// <para>
    /// ASP.NET Core 10 skips descriptions whose HTTP method OpenAPI 3.1 does not know before any operation transformer runs,
    /// and its operation generation is internal. So this document transformer builds each QUERY operation from the
    /// ApiExplorer description itself: path, query and header parameters, the body, and the declared responses. It then
    /// moves every named schema into components and runs the hide-route-parameters transformer the generated operations get.
    /// </para>
    /// </summary>
    public sealed partial class HttpQueryDocumentTransformer : IOpenApiDocumentTransformer
    {
        public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            var descriptions = context.DescriptionGroups
                .SelectMany(g => g.Items)
                .Where(d => HttpMethods.IsQuery(d.HttpMethod ?? string.Empty)
                    && (d.GroupName == null || d.GroupName == context.DocumentName))
                .ToList();

            foreach (var description in descriptions)
            {
                var operation = await CreateOperationAsync(description, document, context, cancellationToken);

                await new HideRouteParametersFromBodyOperationTransformer().TransformAsync(
                    operation,
                    new OpenApiOperationTransformerContext
                    {
                        DocumentName = context.DocumentName,
                        Description = description,
                        ApplicationServices = context.ApplicationServices,
                        Document = document
                    },
                    cancellationToken);

                var path = "/" + RouteParameterConstraint().Replace(description.RelativePath ?? string.Empty, "{$1}");
                document.Paths ??= new OpenApiPaths();
                if (!document.Paths.TryGetValue(path, out var pathItem) || pathItem is not OpenApiPathItem item)
                {
                    item = new OpenApiPathItem();
                    document.Paths[path] = item;
                }

                item.Operations ??= new Dictionary<HttpMethod, OpenApiOperation>();
                // OpenAPI 3.2's native "query" operation; UseHttpQueryOpenApi serves the document labelled 3.2.0.
                item.Operations[HttpMethod.Query] = operation;
            }
        }

        private static async Task<OpenApiOperation> CreateOperationAsync(
            ApiDescription description,
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            var operation = new OpenApiOperation
            {
                Parameters = new List<IOpenApiParameter>(),
                Responses = new OpenApiResponses()
            };

            // Endpoint metadata covers minimal APIs and FastEndpoints; controllers fall back to the controller name.
            var metadata = description.ActionDescriptor.EndpointMetadata;
            var tags = metadata.OfType<ITagsMetadata>().SelectMany(t => t.Tags).Distinct().ToList();
            if (tags.Count == 0 && description.ActionDescriptor is ControllerActionDescriptor controller)
            {
                tags.Add(controller.ControllerName);
            }
            if (tags.Count > 0)
            {
                document.Tags ??= new HashSet<OpenApiTag>();
                foreach (var tag in tags.Where(tag => document.Tags.All(t => t.Name != tag)))
                {
                    document.Tags.Add(new OpenApiTag { Name = tag });
                }
                operation.Tags = new HashSet<OpenApiTagReference>(tags.Select(tag => new OpenApiTagReference(tag, document)));
            }
            operation.Summary = metadata.OfType<IEndpointSummaryMetadata>().LastOrDefault()?.Summary;
            operation.Description = metadata.OfType<IEndpointDescriptionMetadata>().LastOrDefault()?.Description;
            operation.OperationId = metadata.OfType<IEndpointNameMetadata>().LastOrDefault()?.EndpointName;

            foreach (var parameter in description.ParameterDescriptions)
            {
                var location = ParameterLocationFor(parameter.Source);
                if (location is not null)
                {
                    operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = parameter.Name,
                        In = location,
                        Required = location == ParameterLocation.Path || parameter.IsRequired,
                        Schema = Componentize(await context.GetOrCreateSchemaAsync(parameter.Type, parameter, cancellationToken), document)
                    });
                }
                else if (parameter.Source == BindingSource.Body)
                {
                    var schema = Componentize(await context.GetOrCreateSchemaAsync(parameter.Type, parameter, cancellationToken), document);
                    var mediaTypes = description.SupportedRequestFormats.Select(f => f.MediaType).DefaultIfEmpty("application/json");
                    operation.RequestBody = new OpenApiRequestBody
                    {
                        Required = parameter.IsRequired,
                        Content = mediaTypes.Distinct().ToDictionary(m => m, _ => new OpenApiMediaType { Schema = schema })
                    };
                }
            }

            // Endpoints that are not controllers (FastEndpoints) describe no route parameters, so declare any the route template
            // has, as OpenAPI requires every templated path segment to have a parameter.
            foreach (Match match in RouteParameterConstraint().Matches(description.RelativePath ?? string.Empty))
            {
                var name = match.Groups[1].Value;
                if (operation.Parameters.All(p => p.In != ParameterLocation.Path || !string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase)))
                {
                    operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = name,
                        In = ParameterLocation.Path,
                        Required = true,
                        Schema = new OpenApiSchema { Type = JsonSchemaType.String }
                    });
                }
            }

            foreach (var response in description.SupportedResponseTypes)
            {
                var openApiResponse = new OpenApiResponse { Description = ReasonPhrases.GetReasonPhrase(response.StatusCode) };
                if (response.Type is not null && response.Type != typeof(void))
                {
                    var schema = Componentize(await context.GetOrCreateSchemaAsync(response.Type, null, cancellationToken), document);
                    openApiResponse.Content = response.ApiResponseFormats.Select(f => f.MediaType)
                        .DefaultIfEmpty("application/json")
                        .Distinct()
                        .ToDictionary(m => m, _ => new OpenApiMediaType { Schema = schema });
                }
                operation.Responses[response.StatusCode.ToString()] = openApiResponse;
            }

            return operation;
        }

        /// <summary>
        /// GetOrCreateSchemaAsync returns nested types inline. Walk the schema and replace every schema that has a
        /// reference id with a component reference, reusing a component that already exists (e.g. ProductDto from the GET).
        /// </summary>
        private static IOpenApiSchema Componentize(IOpenApiSchema schema, OpenApiDocument document)
        {
            if (schema is not OpenApiSchema concrete)
            {
                return schema;
            }

            if (concrete.Properties is { Count: > 0 } properties)
            {
                foreach (var key in properties.Keys.ToList())
                {
                    properties[key] = Componentize(properties[key], document);
                }
            }
            if (concrete.Items is not null)
            {
                concrete.Items = Componentize(concrete.Items, document);
            }
            if (concrete.AdditionalProperties is not null)
            {
                concrete.AdditionalProperties = Componentize(concrete.AdditionalProperties, document);
            }
            foreach (var list in new[] { concrete.AllOf, concrete.AnyOf, concrete.OneOf })
            {
                if (list is null) continue;
                for (var i = 0; i < list.Count; i++)
                {
                    list[i] = Componentize(list[i], document);
                }
            }

            if (concrete.Metadata?.TryGetValue("x-schema-id", out var value) == true && value is string id && !string.IsNullOrEmpty(id))
            {
                document.Components ??= new OpenApiComponents();
                document.Components.Schemas ??= new Dictionary<string, IOpenApiSchema>();
                document.Components.Schemas.TryAdd(id, concrete);
                return new OpenApiSchemaReference(id, document);
            }

            return concrete;
        }

        private static ParameterLocation? ParameterLocationFor(BindingSource? source)
        {
            if (source == BindingSource.Path) return ParameterLocation.Path;
            if (source == BindingSource.Query) return ParameterLocation.Query;
            if (source == BindingSource.Header) return ParameterLocation.Header;
            return null;
        }

        // "{id:guid}" and "{id?}" become "{id}", as in the generated paths.
        [GeneratedRegex(@"\{([^}:?=]+)[^}]*\}")]
        private static partial Regex RouteParameterConstraint();
    }
}
