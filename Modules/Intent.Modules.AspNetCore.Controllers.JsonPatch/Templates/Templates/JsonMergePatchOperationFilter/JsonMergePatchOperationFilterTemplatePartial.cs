using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.JsonPatch.Templates.Templates.JsonMergePatchOperationFilter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class JsonMergePatchOperationFilterTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Controllers.JsonPatch.Templates.JsonMergePatchOperationFilter";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public JsonMergePatchOperationFilterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.OpenApi")
                .AddUsing("Morcatko.AspNetCore.JsonMergePatch")
                .AddUsing("Swashbuckle.AspNetCore.SwaggerGen")
                .AddClass($"JsonMergePatchOperationFilter", @class =>
                {
                    @class.ImplementsInterface("IOperationFilter");

                    @class.AddMethod("void", "Apply", method =>
                    {
                        method.AddParameter("OpenApiOperation", "operation");
                        method.AddParameter("OperationFilterContext", "context");
                        method.AddStatements(
                            """
                                if (context.ApiDescription.HttpMethod != "PATCH")
                                {
                                    return;
                                }

                                if (operation.RequestBody?.Content == null)
                                {
                                    return;
                                }

                                var patchParam = context.ApiDescription.ParameterDescriptions
                                    .FirstOrDefault(param => param.Type.IsGenericType && param.Type.GetGenericTypeDefinition() == typeof(JsonMergePatchDocument<>));

                                if (patchParam == null)
                                {
                                    return;
                                }

                                var payloadType = patchParam.Type.GetGenericArguments()[0];

                                var payloadSchemaReference = context.SchemaGenerator.GenerateSchema(payloadType, context.SchemaRepository) as OpenApiSchemaReference;

                                if (payloadSchemaReference == null || string.IsNullOrEmpty(payloadSchemaReference.Reference.Id))
                                {
                                    return;
                                }

                                if (context.SchemaRepository.Schemas.TryGetValue(payloadSchemaReference.Reference.Id, out var actualSchema))
                                {
                                    actualSchema?.Required?.Remove("patchExecutor");
                                    actualSchema?.Properties?.Remove("patchExecutor");
                                }

                                if (operation.RequestBody.Content.TryGetValue(JsonMergePatchDocument.ContentType, out var content))
                                {
                                    content.Schema = payloadSchemaReference;
                                }
                                """.ConvertToStatements());
                    });
                });
        }

        // This is a new Swashbuckle Filter so no need to support earlier versions, so if this somehow 
        // runs on an earlier OpenAPI version just don't generate.
        private bool IsMicrosoftOpenApi_2_4_1 => OutputTarget.GetMaxNetAppVersion().Major >= 8;

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() &&
                   IsMicrosoftOpenApi_2_4_1 &&
                   ExecutionContext.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Distribution.SwashbuckleConfiguration")).Any();
        }

        public override void AfterTemplateRegistration()
        {
            if (!IsMicrosoftOpenApi_2_4_1)
            {
                return;
            }

            var templates =
                ExecutionContext.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Distribution.SwashbuckleConfiguration"));

            foreach (var template in templates)
            {
                template.CSharpFile.OnBuild(file =>
                {
                    var cls = file.Classes.First();
                    var configureSwaggerOptionsBlock = GetConfigureSwaggerOptionsBlock(cls);
                    if (configureSwaggerOptionsBlock == null)
                    {
                        return;
                    }

                    configureSwaggerOptionsBlock.AddStatement($@"options.OperationFilter<{template.GetJsonMergePatchOperationFilterName()}>();");
                });
            }
        }

        private static CSharpLambdaBlock? GetConfigureSwaggerOptionsBlock(CSharpClass @class)
        {
            var configureSwaggerMethod = @class.FindMethod("ConfigureSwagger");
            var addSwaggerGen = configureSwaggerMethod?.FindStatement(p => p.HasMetadata("AddSwaggerGen")) as CSharpInvocationStatement;
            var cSharpLambdaBlock = addSwaggerGen?.Statements.First() as CSharpLambdaBlock;
            return cSharpLambdaBlock;
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