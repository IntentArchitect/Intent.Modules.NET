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

namespace Intent.Modules.AspNetCore.Swashbuckle.Templates.HideRouteParametersFromBodyOperationFilter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class HideRouteParametersFromBodyOperationFilterTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Swashbuckle.HideRouteParametersFromBodyOperationFilter";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HideRouteParametersFromBodyOperationFilterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Linq")
                .AddUsing("Microsoft.OpenApi.Models")
                .AddUsing("Swashbuckle.AspNetCore.SwaggerGen")
                .AddClass($"HideRouteParametersFromBodyOperationFilter", @class =>
                {
                    @class.ImplementsInterface("IOperationFilter");
                    @class.WithComments(new[]
                    {
                        "/// <summary>",
                        "/// Operation filter that removes properties from request body schema when they are already defined as route parameters.",
                        "/// This prevents duplicate documentation of parameters that are supplied via the URL.",
                        "/// </summary>"
                    });
                    @class.AddMethod("void", "Apply", method =>
                    {
                        method
                            .AddParameter("OpenApiOperation", "operation")
                            .AddParameter("OperationFilterContext", "context");

                        method.AddStatement("// Only process operations with both route parameters and a request body");
                        method.AddIfStatement("operation.Parameters == null || operation.RequestBody?.Content == null", stmt =>
                        {
                            stmt.AddReturn("");
                            stmt.BeforeSeparator = CSharpCodeSeparatorType.None;
                        });
                        method.AddStatement("// Get all route parameter names (case-insensitive for matching)", c => c.SeparatedFromPrevious());
                        method.AddStatements(
                            """
                            var routeParameters = operation.Parameters
                                .Where(p => p.In == ParameterLocation.Path)
                                .Select(p => p.Name.ToLowerInvariant())
                                .ToHashSet();
                            """.ConvertToStatements());
                        method.AddIfStatement("routeParameters.Count == 0", stmt =>
                        {
                            stmt.AddReturn("");
                        });
                        method.AddStatement("// Process each content type in the request body", c => c.SeparatedFromPrevious());
                        method.AddForEachStatement("contentType", "operation.RequestBody.Content.Keys.ToList()", forEach =>
                        {
                            forEach.BeforeSeparator = CSharpCodeSeparatorType.None;
                            forEach.AddStatement("var content = operation.RequestBody.Content[contentType];");
                            forEach.AddStatement("var schema = content.Schema;");
                            forEach.AddIfStatement("schema == null", stmt =>
                            {
                                stmt.AddStatement("continue;");
                            });
                            forEach.AddStatement("// Handle schema references", s => s.SeparatedFromPrevious());
                            forEach.AddIfStatement("schema.Reference != null", stmt =>
                            {
                                stmt.BeforeSeparator = CSharpCodeSeparatorType.None;
                                stmt.AddStatement("// Get the actual schema from the context");
                                stmt.AddStatement("var schemaRepository = context.SchemaRepository.Schemas;");
                                stmt.AddStatement("var schemaId = schema.Reference.Id;");
                                stmt.AddIfStatement("schemaRepository.TryGetValue(schemaId, out var referencedSchema)", innerStmt =>
                                {
                                    innerStmt.AddStatement("schema = referencedSchema;");
                                });
                            });
                            forEach.AddIfStatement("schema.Properties == null || !schema.Properties.Any()", stmt =>
                            {
                                stmt.AddStatement("continue;");
                            });
                            forEach.AddStatement("// Find properties that match route parameter names (case-insensitive)", s => s.SeparatedFromPrevious());
                            forEach.AddStatements(
                                """
                                var propertiesToRemove = schema.Properties.Keys
                                    .Where(key => routeParameters.Contains(key.ToLowerInvariant()))
                                    .ToList();
                                """.ConvertToStatements());
                            forEach.AddIfStatement("propertiesToRemove.Count == 0", stmt =>
                            {
                                stmt.AddStatement("continue;");
                            });
                            forEach.AddStatement("// Create a new schema with the filtered properties", s => s.SeparatedFromPrevious());
                            forEach.AddStatement("var newSchema = new OpenApiSchema(schema);");
                            forEach.AddStatement("// Remove matching properties from the new schema", s => s.SeparatedFromPrevious());
                            forEach.AddForEachStatement("propertyName", "propertiesToRemove", innerForEach =>
                            {
                                innerForEach.BeforeSeparator = CSharpCodeSeparatorType.None;
                                innerForEach.AddStatement("newSchema.Properties.Remove(propertyName);");
                                innerForEach.AddStatement("newSchema.Required.Remove(propertyName);");
                            });
                            forEach.AddStatement("// Replace the content schema with the new filtered schema", s => s.SeparatedFromPrevious());
                            forEach.AddStatement("operation.RequestBody.Content[contentType].Schema = newSchema;");
                        });
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() &&
                   ExecutionContext.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Distribution.SwashbuckleConfiguration")).Any();
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
