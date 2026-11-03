using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Scalar.Templates.HttpQueryDocumentTransformer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class HttpQueryDocumentTransformerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Scalar.HttpQueryDocumentTransformer";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HttpQueryDocumentTransformerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Net.Http")
                .AddUsing("System.Text.RegularExpressions")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.AspNetCore.Http")
                .AddUsing("Microsoft.AspNetCore.Http.Metadata")
                .AddUsing("Microsoft.AspNetCore.Mvc.ApiExplorer")
                .AddUsing("Microsoft.AspNetCore.Mvc.Controllers")
                .AddUsing("Microsoft.AspNetCore.Mvc.ModelBinding")
                .AddUsing("Microsoft.AspNetCore.OpenApi")
                .AddUsing("Microsoft.AspNetCore.WebUtilities")
                .AddUsing("Microsoft.OpenApi")
                .AddClass("HttpQueryDocumentTransformer", @class =>
                {
                    @class.Sealed();
                    @class.ImplementsInterface("IOpenApiDocumentTransformer");
                    @class.WithComments(
                    [
                        "/// <summary>",
                        "/// Adds HTTP QUERY operations to the Microsoft.AspNetCore.OpenApi document.",
                        "/// <para>",
                        "/// ASP.NET Core 10 skips descriptions whose HTTP method OpenAPI 3.1 does not know before any operation transformer runs,",
                        "/// and its operation generation is internal. So this document transformer builds each QUERY operation from the",
                        "/// ApiExplorer description itself: path, query and header parameters, the body, and the declared responses. It then",
                        "/// moves every named schema into components and runs the hide-route-parameters transformer the generated operations get.",
                        "/// </para>",
                        "/// </summary>"
                    ]);

                    @class.AddField("Regex", "RouteParameterConstraint", field =>
                    {
                        // "{id:guid}" and "{id?}" become "{id}", as in the generated paths.
                        field.PrivateReadOnly().Static();
                        field.WithAssignment("""new(@"\{([^}:?=]+)[^}]*\}")""");
                    });

                    @class.AddMethod("Task<OpenApiOperation>", "CreateOperationAsync", method =>
                    {
                        method.Private().Static().Async();
                        method.AddParameter("ApiDescription", "description");
                        method.AddParameter("OpenApiDocument", "document");
                        method.AddParameter("OpenApiDocumentTransformerContext", "context");
                        method.AddParameter("CancellationToken", "cancellationToken");
                        method.AddStatement("""
                            var operation = new OpenApiOperation
                            {
                                Parameters = new List<IOpenApiParameter>(),
                                Responses = new OpenApiResponses()
                            };
                            """);
                        method.AddStatement("""
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
                            """, s => s.SeparatedFromPrevious());
                        method.AddStatement("""
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
                            """, s => s.SeparatedFromPrevious());
                        method.AddStatement("""
                            // Endpoints that are not controllers (FastEndpoints) describe no route parameters, so declare any the route template
                            // has, as OpenAPI requires every templated path segment to have a parameter.
                            foreach (Match match in RouteParameterConstraint.Matches(description.RelativePath ?? string.Empty))
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
                            """, s => s.SeparatedFromPrevious());
                        method.AddStatement("""
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
                            """, s => s.SeparatedFromPrevious());
                        method.AddStatement("return operation;", s => s.SeparatedFromPrevious());
                    });

                    @class.AddMethod("IOpenApiSchema", "Componentize", method =>
                    {
                        method.Private().Static();
                        method.WithComments(
                        [
                            "/// <summary>",
                            "/// GetOrCreateSchemaAsync returns nested types inline. Walk the schema and replace every schema that has a",
                            "/// reference id with a component reference, reusing a component that already exists (e.g. ProductDto from the GET).",
                            "/// </summary>"
                        ]);
                        method.AddParameter("IOpenApiSchema", "schema");
                        method.AddParameter("OpenApiDocument", "document");
                        method.AddStatement("""
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
                            """);
                    });

                    @class.AddMethod("ParameterLocation?", "ParameterLocationFor", method =>
                    {
                        method.Private().Static();
                        method.AddParameter("BindingSource?", "source");
                        method.AddStatement("if (source == BindingSource.Path) return ParameterLocation.Path;");
                        method.AddStatement("if (source == BindingSource.Query) return ParameterLocation.Query;");
                        method.AddStatement("if (source == BindingSource.Header) return ParameterLocation.Header;");
                        method.AddStatement("return null;");
                    });

                })
                .OnBuild(file =>
                {
                    // The hide-route-parameters transformer is in this module, resolved once every template is registered.
                    var hideTransformerName = this.GetHideRouteParametersFromBodyOperationTransformerName();

                    file.Classes.First().AddMethod("Task", "TransformAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("OpenApiDocument", "document");
                        method.AddParameter("OpenApiDocumentTransformerContext", "context");
                        method.AddParameter("CancellationToken", "cancellationToken");
                        method.AddStatement("""
                            var descriptions = context.DescriptionGroups
                                .SelectMany(g => g.Items)
                                .Where(d => HttpMethods.IsQuery(d.HttpMethod ?? string.Empty)
                                    && (d.GroupName == null || d.GroupName == context.DocumentName))
                                .ToList();
                            """);
                        method.AddForEachStatement("description", "descriptions", @foreach =>
                        {
                            @foreach.AddStatement("var operation = await CreateOperationAsync(description, document, context, cancellationToken);");
                            @foreach.AddStatement($$"""
                                await new {{hideTransformerName}}().TransformAsync(
                                    operation,
                                    new OpenApiOperationTransformerContext
                                    {
                                        DocumentName = context.DocumentName,
                                        Description = description,
                                        ApplicationServices = context.ApplicationServices,
                                        Document = document
                                    },
                                    cancellationToken);
                                """, s => s.SeparatedFromPrevious());
                            @foreach.AddStatement("""
                                var path = "/" + RouteParameterConstraint.Replace(description.RelativePath ?? string.Empty, "{$1}");
                                document.Paths ??= new OpenApiPaths();
                                if (!document.Paths.TryGetValue(path, out var pathItem) || pathItem is not OpenApiPathItem item)
                                {
                                    item = new OpenApiPathItem();
                                    document.Paths[path] = item;
                                }
                                """, s => s.SeparatedFromPrevious());
                            @foreach.AddStatement("""
                                item.Operations ??= new Dictionary<HttpMethod, OpenApiOperation>();
                                // OpenAPI 3.2's native "query" operation; UseHttpQueryOpenApi serves the document labelled 3.2.0.
                                item.Operations[HttpMethod.Query] = operation;
                                """, s => s.SeparatedFromPrevious());
                        });
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && HttpQueryHelper.NeedsHttpQueryOpenApi(OutputTarget);
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}