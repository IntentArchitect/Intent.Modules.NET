using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Scalar.Templates.HideRouteParametersFromBodyOperationTransformer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class HideRouteParametersFromBodyOperationTransformerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Scalar.HideRouteParametersFromBodyOperationTransformer";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HideRouteParametersFromBodyOperationTransformerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var usesMicrosoftOpenApiV2 = OutputTarget.GetProject().GetMaxNetAppVersion().Major >= 10;

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Linq")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.AspNetCore.OpenApi")
                .AddClass($"HideRouteParametersFromBodyOperationTransformer", @class =>
                {
                    if (usesMicrosoftOpenApiV2)
                    {
                        AddUsing("Microsoft.OpenApi");
                        @class.ImplementsInterface("IOpenApiOperationTransformer");
                    }
                    else
                    {
                        AddUsing("Microsoft.OpenApi.Models");
                        @class.ImplementsInterface("IOpenApiOperationTransformer");
                    }

                    @class.WithComments(new[]
                    {
                        "/// <summary>",
                        "/// Operation transformer that removes properties from request body schema when they are already defined as route parameters.",
                        "/// This prevents duplicate documentation of parameters that are supplied via the URL.",
                        "/// </summary>"
                    });

                    @class.AddMethod("Task", "TransformAsync", method =>
                    {
                        method
                            .AddParameter("OpenApiOperation", "operation")
                            .AddParameter("OpenApiOperationTransformerContext", "context")
                            .AddParameter("CancellationToken", "cancellationToken");

                        method.AddStatement("// Only process operations with both route parameters and a request body");
                        method.AddIfStatement("operation.Parameters == null || operation.RequestBody?.Content == null", stmt =>
                        {
                            stmt.BeforeSeparator = CSharpCodeSeparatorType.None;
                            stmt.AddStatement("return Task.CompletedTask;");
                        });
                        method.AddStatement("// Get all route parameter names (case-insensitive for matching)", c => c.SeparatedFromPrevious());
                        method.AddStatements(
                            """
                            var routeParameters = operation.Parameters
                                .Where(p => p.In == ParameterLocation.Path)
                                .Select(p => p.Name?.ToLowerInvariant())
                                .Where(p => !string.IsNullOrEmpty(p))
                                .ToHashSet();
                            """.ConvertToStatements());
                        method.AddIfStatement("routeParameters.Count == 0", stmt =>
                        {
                            stmt.AddStatement("return Task.CompletedTask;");
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
                            if (!usesMicrosoftOpenApiV2)
                            {
                                // For .NET 9 - skip reference resolution as context doesn't provide document access
                                forEach.AddStatement("// Note: Schema references are typically already resolved at this stage", s => s.SeparatedFromPrevious());
                                forEach.AddIfStatement("schema.Reference != null", stmt =>
                                {
                                    stmt.BeforeSeparator = CSharpCodeSeparatorType.None;
                                    stmt.AddStatement("// Schema is a reference, properties may not be directly accessible");
                                    stmt.AddStatement("continue;");
                                });
                            }

                            forEach.AddIfStatement("schema.Properties == null || schema.Properties.Count == 0", stmt =>
                            {
                                stmt.AddStatement("continue;");
                            });
                            forEach.AddStatement("// Find properties that match route parameter names (case-insensitive)", s => s.SeparatedFromPrevious());
                            forEach.AddStatements(
                                """
                                var propertiesToRemove = schema.Properties.Keys
                                    .Where(key => routeParameters.Contains(key?.ToLowerInvariant()))
                                    .ToList();
                                """.ConvertToStatements());
                            forEach.AddIfStatement("propertiesToRemove.Count == 0", stmt =>
                            {
                                stmt.AddStatement("continue;");
                            });

                            if (usesMicrosoftOpenApiV2)
                            {
                                // For .NET 10+ - OpenAPI v2 approach
                                forEach.AddStatement("// Remove matching properties directly from the schema", c => c.SeparatedFromPrevious());
                                forEach.AddForEachStatement("propertyName", "propertiesToRemove", innerForEach =>
                                {
                                    innerForEach.BeforeSeparator = CSharpCodeSeparatorType.None;
                                    innerForEach.AddIfStatement("propertyName != null", ifStmt =>
                                    {
                                        ifStmt.AddStatement("schema.Properties.Remove(propertyName);");
                                        ifStmt.AddStatement("schema.Required?.Remove(propertyName);");
                                    });
                                });
                            }
                            else
                            {
                                // For .NET 9 - OpenAPI v1 approach with schema cloning
                                forEach.AddStatement("// Create a new schema with the filtered properties", s => s.SeparatedFromPrevious());
                                forEach.AddStatement("var newSchema = new OpenApiSchema(schema);");
                                forEach.AddStatement("// Remove matching properties from the new schema", s => s.SeparatedFromPrevious());
                                forEach.AddForEachStatement("propertyName", "propertiesToRemove", innerForEach =>
                                {
                                    innerForEach.BeforeSeparator = CSharpCodeSeparatorType.None;
                                    innerForEach.AddStatement("newSchema.Properties.Remove(propertyName);");
                                    innerForEach.AddStatement("newSchema.Required?.Remove(propertyName);");
                                });
                                forEach.AddStatement("// Replace the content schema with the new filtered schema", s => s.SeparatedFromPrevious());
                                forEach.AddStatement("operation.RequestBody.Content[contentType].Schema = newSchema;");
                            }
                        });
                        method.AddStatement("return Task.CompletedTask;", m => m.SeparatedFromPrevious());
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() &&
                   ExecutionContext.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Intent.AspNetCore.Scalar.OpenApiConfiguration")).Any();
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
