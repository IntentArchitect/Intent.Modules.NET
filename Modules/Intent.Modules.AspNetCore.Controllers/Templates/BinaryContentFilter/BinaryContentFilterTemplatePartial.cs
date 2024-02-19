using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Newtonsoft.Json;

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
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"BinaryContentFilter", @class =>
                {
                    AddUsing("System.Linq");
                    AddUsing("Microsoft.OpenApi.Models");
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

                        method.AddStatements(@"operation.Parameters.Add(new OpenApiParameter
            {
                Name = ""Content-Disposition"",
                In = ParameterLocation.Header,
                Required = false,
                Schema = new OpenApiSchema
                {                    
                    Type = ""string""
                },
                Description = ""e.g. form-data; name=\""file\""; filename=example.txt""
            });
".ConvertToStatements());
                        method.AddStatement("operation.RequestBody = new OpenApiRequestBody() {Required = true};");
                        method.AddStatement(@"operation.RequestBody.Content.Add(""application/octet-stream"", new OpenApiMediaType()
            {
                Schema = new OpenApiSchema()
                {
                    Type = ""string"",
                    Format = ""binary"",
                },
            });");

                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() &&
                FileTransferHelper.NeedsFileUploadInfrastructure(ExecutionContext.MetadataManager, ExecutionContext.GetApplicationConfig().Id) &&
                ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Distribution.SwashbuckleConfiguration")) != null;
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