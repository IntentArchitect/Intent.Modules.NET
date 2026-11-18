using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Newtonsoft.Json;
using NuGet.Versioning;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Templates.BinaryContentFilter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class BinaryContentFilterTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Controllers.BinaryContentFilter";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public BinaryContentFilterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var isMicrosoftOpenApi_2_4_1 = OutputTarget.GetMaxNetAppVersion().Major >= 8;
            var openApiModelsNamespace = isMicrosoftOpenApi_2_4_1 ? "Microsoft.OpenApi" : "Microsoft.OpenApi.Models";
            // Swashbuckle 11 depends on Microsoft.OpenApi 3.x, which types request body content as IOpenApiMediaType.
            // The filter only generates alongside Intent.AspNetCore.Swashbuckle, which registers the package.
            var usesMicrosoftOpenApiV3 = isMicrosoftOpenApi_2_4_1 &&
                                         ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.AspNetCore.Swashbuckle") &&
                                         NuGetVersion.Parse(NugetRegistry.GetVersion("Swashbuckle.AspNetCore", OutputTarget.GetMaxNetAppVersion()).Version).Major >= 11;
            var mediaTypeValueType = usesMicrosoftOpenApiV3 ? "IOpenApiMediaType" : "OpenApiMediaType";

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"BinaryContentFilter", @class =>
                {
                    AddUsing("System.Linq");
                    AddUsing(openApiModelsNamespace);
                    AddUsing("Swashbuckle.AspNetCore.SwaggerGen");
                    @class.ImplementsInterface(UseType("Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter"));

                    @class.AddMethod("void", "Apply", method =>
                    {
                        method.WithComments(new[]
                            {
                        "/// <summary>",
                        $"/// Configures operations decorated with the <see cref=\"{this.GetBinaryContentAttributeName()}\" />.",
                        "/// </summary>",
                        "/// <param name=\"operation\">The operation.</param>",
                        "/// <param name=\"context\">The context.</param>",
                            });


                        method
                            .AddParameter("OpenApiOperation", "operation")
                            .AddParameter("OperationFilterContext", "context");
                        method.AddStatement($"var attribute = context.MethodInfo.GetCustomAttributes(typeof({this.GetBinaryContentAttributeName()}), false).FirstOrDefault();");
                        method.AddIfStatement("attribute == null", stmt =>
                        {
                            stmt.AddStatement("return;");
                        });

                        if (usesMicrosoftOpenApiV3)
                        {
                            // Microsoft.OpenApi 3.x types Parameters as nullable
                            method.AddStatement("operation.Parameters ??= [];");
                        }

                        if (isMicrosoftOpenApi_2_4_1)
                        {
                            method.AddStatements(
                                """
                                operation.Parameters.Add(new OpenApiParameter
                                {
                                    Name = "Content-Disposition",
                                    In = ParameterLocation.Header,
                                    Required = false,
                                    Schema = new OpenApiSchema
                                    {                    
                                        Type = JsonSchemaType.String
                                    },
                                    Description = "e.g. form-data; name=\"file\"; filename=example.txt"
                                });
                                """.ConvertToStatements());
                        }
                        else
                        {
                            method.AddStatements(
                                """
                                operation.Parameters.Add(new OpenApiParameter
                                {
                                    Name = "Content-Disposition",
                                    In = ParameterLocation.Header,
                                    Required = false,
                                    Schema = new OpenApiSchema
                                    {                    
                                        Type = "string"
                                    },
                                    Description = "e.g. form-data; name=\"file\"; filename=example.txt"
                                });
                                """.ConvertToStatements());
                        }

                        AddUsing("System.Collections.Generic");

                        method.AddStatement($@"operation.RequestBody = new OpenApiRequestBody()
            {{
                Required = true,
                Content = new Dictionary<string, {mediaTypeValueType}>()
            }};");

                        if (isMicrosoftOpenApi_2_4_1)
                        {
                            method.AddStatements(
                                """
                                operation.RequestBody.Content!.Add("application/octet-stream", new OpenApiMediaType()
                                {
                                    Schema = new OpenApiSchema()
                                    {
                                        Type = JsonSchemaType.String,
                                        Format = "binary",
                                    },
                                });
                                """.ConvertToStatements());
                        }
                        else
                        {
                            method.AddStatements(
                                """
                                operation.RequestBody.Content!.Add("application/octet-stream", new OpenApiMediaType()
                                {
                                    Schema = new OpenApiSchema()
                                    {
                                        Type = "string",
                                        Format = "binary",
                                    },
                                });
                                """.ConvertToStatements());
                        }
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() &&
                FileTransferHelper.NeedsFileUploadInfrastructure(ExecutionContext.MetadataManager, ExecutionContext.GetApplicationConfig().Id) &&
                // Scoped to THIS output target: only generate the filter into a host that itself has a
                // Swashbuckle configuration. The IOutputTarget string overload defaults accessibleTo to
                // the receiver, so this stays correct when an application hosts several web projects.
                OutputTarget.FindTemplateInstance<ICSharpFileBuilderTemplate>("Distribution.SwashbuckleConfiguration") != null;
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