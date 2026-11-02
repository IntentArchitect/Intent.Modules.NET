using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HttpQuery.Controllers.Swashbuckle.Api.Filters
{
    /// <summary>
    /// Adds HTTP QUERY operations to the Swashbuckle document.
    /// <para>
    /// Swashbuckle 10 throws for any HTTP method outside GET/PUT/POST/DELETE/OPTIONS/HEAD/PATCH/TRACE, so
    /// <see cref="ExcludeFromMainPass"/> keeps QUERY descriptions out of the main pass. This filter then runs copies of
    /// them, re-labelled as POST, through a second <see cref="SwaggerGenerator"/> built from the same options, so every
    /// registered operation filter, XML comment and response type still applies, and moves each resulting operation into
    /// the path item's QUERY slot.
    /// </para>
    /// </summary>
    public class HttpQueryDocumentFilter : IDocumentFilter
    {
        private static readonly AsyncLocal<bool> RunningNested = new();
        private readonly IServiceProvider _serviceProvider;

        // Only the service provider is taken here: Swashbuckle creates filters while it is still building
        // SwaggerGeneratorOptions, so asking for the options in the constructor would re-enter that setup.
        public HttpQueryDocumentFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// The DocInclusionPredicate: Swashbuckle's default group match, minus QUERY descriptions.
        /// </summary>
        public static bool ExcludeFromMainPass(string documentName, ApiDescription apiDescription)
        {
            return !IsQuery(apiDescription) && IncludedInDocument(documentName, apiDescription);
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // The nested generator shares these options, so it runs this filter too.
            if (RunningNested.Value)
            {
                return;
            }

            var documentName = context.DocumentName;
            var queryDescriptions = _serviceProvider.GetRequiredService<IApiDescriptionGroupCollectionProvider>()
                .ApiDescriptionGroups.Items
                .SelectMany(g => g.Items)
                .Where(d => IsQuery(d) && IncludedInDocument(documentName, d))
                .ToList();
            if (queryDescriptions.Count == 0)
            {
                return;
            }

            var nestedDocument = GenerateAsPost(documentName, queryDescriptions);
            MergeQueryOperations(swaggerDoc, nestedDocument);
        }

        private OpenApiDocument GenerateAsPost(string documentName, IReadOnlyList<ApiDescription> queryDescriptions)
        {
            var options = _serviceProvider.GetRequiredService<IOptions<SwaggerGeneratorOptions>>().Value;
            var schemaGenerator = _serviceProvider.GetRequiredService<ISchemaGenerator>();
            var provider = new FixedApiDescriptionProvider(queryDescriptions.Select(CopyAsPost).ToList());
            var generator = new SwaggerGenerator(options, provider, schemaGenerator);

            RunningNested.Value = true;
            try
            {
                return generator.GetSwagger(documentName);
            }
            finally
            {
                RunningNested.Value = false;
            }
        }

        private static void MergeQueryOperations(OpenApiDocument target, OpenApiDocument source)
        {
            target.Paths ??= new OpenApiPaths();
            foreach (var (path, sourcePathItem) in source.Paths)
            {
                if (sourcePathItem is not OpenApiPathItem sourceItem || sourceItem.Operations is null
                    || !sourceItem.Operations.TryGetValue(HttpMethod.Post, out var operation))
                {
                    continue;
                }

                if (!target.Paths.TryGetValue(path, out var targetPathItem) || targetPathItem is not OpenApiPathItem targetItem)
                {
                    targetItem = new OpenApiPathItem();
                    target.Paths[path] = targetItem;
                }

                RebindTags(operation, target);
                // OpenAPI 3.2's native "query" operation. Microsoft.OpenApi 2.x writes any operation key lower-cased;
                // OpenApi32DocumentSerializer labels the document 3.2.0 so readers and UIs accept the key.
                targetItem.Operations ??= new Dictionary<HttpMethod, OpenApiOperation>();
                targetItem.Operations[HttpMethod.Query] = operation;
            }

            if (source.Components?.Schemas is { } schemas)
            {
                target.Components ??= new OpenApiComponents();
                target.Components.Schemas ??= new Dictionary<string, IOpenApiSchema>();
                foreach (var (id, schema) in schemas)
                {
                    target.Components.Schemas.TryAdd(id, schema);
                }
            }

            if (source.Components?.SecuritySchemes is { } schemes)
            {
                target.Components ??= new OpenApiComponents();
                target.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                foreach (var (id, scheme) in schemes)
                {
                    target.Components.SecuritySchemes.TryAdd(id, scheme);
                }
            }

            if (source.Tags is { } tags)
            {
                target.Tags ??= new HashSet<OpenApiTag>();
                foreach (var tag in tags.Where(t => target.Tags.All(existing => existing.Name != t.Name)))
                {
                    target.Tags.Add(tag);
                }
            }
        }

        // Tag references point at the document they were created for; point them at the main document.
        private static void RebindTags(OpenApiOperation operation, OpenApiDocument target)
        {
            if (operation.Tags is { Count: > 0 } tags)
            {
                operation.Tags = new HashSet<OpenApiTagReference>(tags.Select(t => new OpenApiTagReference(t.Name!, target)));
            }
        }

        // The description provider caches its descriptions, so the originals are copied rather than changed.
        private static ApiDescription CopyAsPost(ApiDescription original)
        {
            var copy = new ApiDescription
            {
                ActionDescriptor = original.ActionDescriptor,
                GroupName = original.GroupName,
                HttpMethod = HttpMethods.Post,
                RelativePath = original.RelativePath
            };
            foreach (var parameter in original.ParameterDescriptions)
            {
                copy.ParameterDescriptions.Add(parameter);
            }
            foreach (var format in original.SupportedRequestFormats)
            {
                copy.SupportedRequestFormats.Add(format);
            }
            foreach (var responseType in original.SupportedResponseTypes)
            {
                copy.SupportedResponseTypes.Add(responseType);
            }
            // Properties carries the Asp.Versioning ApiVersion.
            foreach (var (key, value) in original.Properties)
            {
                copy.Properties[key] = value;
            }
            return copy;
        }

        private static bool IsQuery(ApiDescription apiDescription)
        {
            return HttpMethods.IsQuery(apiDescription.HttpMethod ?? string.Empty);
        }

        private static bool IncludedInDocument(string documentName, ApiDescription apiDescription)
        {
            return apiDescription.GroupName == null || apiDescription.GroupName == documentName;
        }

        private sealed class FixedApiDescriptionProvider : IApiDescriptionGroupCollectionProvider
        {
            public FixedApiDescriptionProvider(IReadOnlyList<ApiDescription> descriptions)
            {
                ApiDescriptionGroups = new ApiDescriptionGroupCollection(
                    descriptions.GroupBy(d => d.GroupName)
                        .Select(g => new ApiDescriptionGroup(g.Key, g.ToList()))
                        .ToList(),
                    version: 1);
            }

            public ApiDescriptionGroupCollection ApiDescriptionGroups { get; }
        }
    }
}
