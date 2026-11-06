using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Swashbuckle.Templates.HttpQueryDocumentFilter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class HttpQueryDocumentFilterTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Swashbuckle.HttpQueryDocumentFilter";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HttpQueryDocumentFilterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Net.Http")
                .AddUsing("System.Threading")
                .AddUsing("Microsoft.AspNetCore.Http")
                .AddUsing("Microsoft.AspNetCore.Mvc.ApiExplorer")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Extensions.Options")
                .AddUsing("Microsoft.OpenApi")
                .AddUsing("Swashbuckle.AspNetCore.SwaggerGen")
                .AddClass("HttpQueryDocumentFilter", @class =>
                {
                    @class.ImplementsInterface("IDocumentFilter");
                    @class.WithComments(
                    [
                        "/// <summary>",
                        "/// Adds HTTP QUERY operations to the Swashbuckle document.",
                        "/// <para>",
                        "/// Swashbuckle 10 throws for any HTTP method outside GET/PUT/POST/DELETE/OPTIONS/HEAD/PATCH/TRACE, so",
                        "/// <see cref=\"ExcludeFromMainPass\"/> keeps QUERY descriptions out of the main pass. This filter then runs copies of",
                        "/// them, re-labelled as POST, through a second <see cref=\"SwaggerGenerator\"/> built from the same options, so every",
                        "/// registered operation filter, XML comment and response type still applies, and moves each resulting operation into",
                        "/// the path item's QUERY slot.",
                        "/// </para>",
                        "/// </summary>"
                    ]);

                    @class.AddField("AsyncLocal<bool>", "RunningNested", field =>
                    {
                        field.Private().Static().ReadOnly();
                        field.WithAssignment("new()");
                    });

                    // Only the service provider is taken: Swashbuckle creates filters while it is still building
                    // SwaggerGeneratorOptions, so asking for the options in the constructor would re-enter that setup.
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("IServiceProvider", "serviceProvider", param => param.IntroduceReadonlyField());
                    });

                    @class.AddMethod("bool", "ExcludeFromMainPass", method =>
                    {
                        method.Static();
                        method.WithComments(
                        [
                            "/// <summary>",
                            "/// The DocInclusionPredicate: Swashbuckle's default group match, minus QUERY descriptions.",
                            "/// </summary>"
                        ]);
                        method.AddParameter("string", "documentName");
                        method.AddParameter("ApiDescription", "apiDescription");
                        method.AddStatement("return !IsQuery(apiDescription) && IncludedInDocument(documentName, apiDescription);");
                    });

                    @class.AddMethod("void", "Apply", method =>
                    {
                        method.AddParameter("OpenApiDocument", "swaggerDoc");
                        method.AddParameter("DocumentFilterContext", "context");
                        method.AddIfStatement("RunningNested.Value", @if =>
                        {
                            @if.AddStatement("// The nested generator shares these options, so it runs this filter too.");
                            @if.AddStatement("return;");
                        });
                        method.AddStatement("var documentName = context.DocumentName;", s => s.SeparatedFromPrevious());
                        method.AddStatement("""
                            var queryDescriptions = _serviceProvider.GetRequiredService<IApiDescriptionGroupCollectionProvider>()
                                .ApiDescriptionGroups.Items
                                .SelectMany(g => g.Items)
                                .Where(d => IsQuery(d) && IncludedInDocument(documentName, d))
                                .ToList();
                            """);
                        method.AddIfStatement("queryDescriptions.Count == 0", @if => @if.AddStatement("return;"));
                        method.AddStatement("var nestedDocument = GenerateAsPost(documentName, queryDescriptions);", s => s.SeparatedFromPrevious());
                        method.AddStatement("MergeQueryOperations(swaggerDoc, nestedDocument);");
                    });

                    @class.AddMethod("OpenApiDocument", "GenerateAsPost", method =>
                    {
                        method.Private();
                        method.AddParameter("string", "documentName");
                        method.AddParameter("IReadOnlyList<ApiDescription>", "queryDescriptions");
                        method.AddStatement("var options = _serviceProvider.GetRequiredService<IOptions<SwaggerGeneratorOptions>>().Value;");
                        method.AddStatement("var schemaGenerator = _serviceProvider.GetRequiredService<ISchemaGenerator>();");
                        method.AddStatement("var provider = new FixedApiDescriptionProvider(queryDescriptions.Select(CopyAsPost).ToList());");
                        method.AddStatement("var generator = new SwaggerGenerator(options, provider, schemaGenerator);");
                        method.AddStatement("RunningNested.Value = true;", s => s.SeparatedFromPrevious());
                        method.AddTryBlock(@try => @try.AddStatement("return generator.GetSwagger(documentName);"))
                            .AddFinallyBlock(@finally => @finally.AddStatement("RunningNested.Value = false;"));
                    });

                    @class.AddMethod("void", "MergeQueryOperations", method =>
                    {
                        method.Private().Static();
                        method.AddParameter("OpenApiDocument", "target");
                        method.AddParameter("OpenApiDocument", "source");
                        method.AddStatement("target.Paths ??= new OpenApiPaths();");
                        method.AddForEachStatement("(path, sourcePathItem)", "source.Paths", @foreach =>
                        {
                            @foreach.AddIfStatement("""
                                sourcePathItem is not OpenApiPathItem sourceItem || sourceItem.Operations is null
                                    || !sourceItem.Operations.TryGetValue(HttpMethod.Post, out var operation)
                                """, @if => @if.AddStatement("continue;"));
                            @foreach.AddIfStatement("!target.Paths.TryGetValue(path, out var targetPathItem) || targetPathItem is not OpenApiPathItem targetItem", @if =>
                            {
                                @if.AddStatement("targetItem = new OpenApiPathItem();");
                                @if.AddStatement("target.Paths[path] = targetItem;");
                                @if.SeparatedFromPrevious();
                            });
                            @foreach.AddStatement("RebindTags(operation, target);", s => s.SeparatedFromPrevious());
                            @foreach.AddStatement("""
                                // OpenAPI 3.2's native "query" operation. Microsoft.OpenApi 2.x writes any operation key lower-cased;
                                // OpenApi32DocumentSerializer labels the document 3.2.0 so readers and UIs accept the key.
                                targetItem.Operations ??= new Dictionary<HttpMethod, OpenApiOperation>();
                                targetItem.Operations[HttpMethod.Query] = operation;
                                """);
                        });

                        method.AddIfStatement("source.Components?.Schemas is { } schemas", @if =>
                        {
                            @if.AddStatement("target.Components ??= new OpenApiComponents();");
                            @if.AddStatement("target.Components.Schemas ??= new Dictionary<string, IOpenApiSchema>();");
                            @if.AddForEachStatement("(id, schema)", "schemas", @foreach => @foreach.AddStatement("target.Components.Schemas.TryAdd(id, schema);"));
                            @if.SeparatedFromPrevious();
                        });

                        method.AddIfStatement("source.Components?.SecuritySchemes is { } schemes", @if =>
                        {
                            @if.AddStatement("target.Components ??= new OpenApiComponents();");
                            @if.AddStatement("target.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();");
                            @if.AddForEachStatement("(id, scheme)", "schemes", @foreach => @foreach.AddStatement("target.Components.SecuritySchemes.TryAdd(id, scheme);"));
                        });

                        method.AddIfStatement("source.Tags is { } tags", @if =>
                        {
                            @if.AddStatement("target.Tags ??= new HashSet<OpenApiTag>();");
                            @if.AddForEachStatement("tag", "tags.Where(t => target.Tags.All(existing => existing.Name != t.Name))", @foreach => @foreach.AddStatement("target.Tags.Add(tag);"));
                        });
                    });

                    @class.AddMethod("void", "RebindTags", method =>
                    {
                        method.Private().Static();
                        method.WithComments("// Tag references point at the document they were created for; point them at the main document.");
                        method.AddParameter("OpenApiOperation", "operation");
                        method.AddParameter("OpenApiDocument", "target");
                        method.AddIfStatement("operation.Tags is { Count: > 0 } tags", @if =>
                        {
                            @if.AddStatement("operation.Tags = new HashSet<OpenApiTagReference>(tags.Select(t => new OpenApiTagReference(t.Name!, target)));");
                        });
                    });

                    @class.AddMethod("ApiDescription", "CopyAsPost", method =>
                    {
                        method.Private().Static();
                        method.WithComments("// The description provider caches its descriptions, so the originals are copied rather than changed.");
                        method.AddParameter("ApiDescription", "original");
                        method.AddStatement("""
                            var copy = new ApiDescription
                            {
                                ActionDescriptor = original.ActionDescriptor,
                                GroupName = original.GroupName,
                                HttpMethod = HttpMethods.Post,
                                RelativePath = original.RelativePath
                            };
                            """);
                        method.AddForEachStatement("parameter", "original.ParameterDescriptions", @foreach => @foreach.AddStatement("copy.ParameterDescriptions.Add(parameter);"));
                        method.AddForEachStatement("format", "original.SupportedRequestFormats", @foreach => @foreach.AddStatement("copy.SupportedRequestFormats.Add(format);"));
                        method.AddForEachStatement("responseType", "original.SupportedResponseTypes", @foreach => @foreach.AddStatement("copy.SupportedResponseTypes.Add(responseType);"));
                        method.AddStatement("// Properties carries the Asp.Versioning ApiVersion.");
                        method.AddForEachStatement("(key, value)", "original.Properties", @foreach => @foreach.AddStatement("copy.Properties[key] = value;"));
                        method.AddStatement("return copy;", s => s.SeparatedFromPrevious());
                    });

                    @class.AddMethod("bool", "IsQuery", method =>
                    {
                        method.Private().Static();
                        method.AddParameter("ApiDescription", "apiDescription");
                        method.AddStatement("return HttpMethods.IsQuery(apiDescription.HttpMethod ?? string.Empty);");
                    });

                    @class.AddMethod("bool", "IncludedInDocument", method =>
                    {
                        method.Private().Static();
                        method.AddParameter("string", "documentName");
                        method.AddParameter("ApiDescription", "apiDescription");
                        method.AddStatement("return apiDescription.GroupName == null || apiDescription.GroupName == documentName;");
                    });

                    @class.AddNestedClass("FixedApiDescriptionProvider", nested =>
                    {
                        nested.Private().Sealed();
                        nested.ImplementsInterface("IApiDescriptionGroupCollectionProvider");
                        nested.AddConstructor(ctor =>
                        {
                            ctor.AddParameter("IReadOnlyList<ApiDescription>", "descriptions");
                            ctor.AddStatement("""
                                ApiDescriptionGroups = new ApiDescriptionGroupCollection(
                                    descriptions.GroupBy(d => d.GroupName)
                                        .Select(g => new ApiDescriptionGroup(g.Key, g.ToList()))
                                        .ToList(),
                                    version: 1);
                                """);
                        });
                        nested.AddProperty("ApiDescriptionGroupCollection", "ApiDescriptionGroups", property => property.WithoutSetter());
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