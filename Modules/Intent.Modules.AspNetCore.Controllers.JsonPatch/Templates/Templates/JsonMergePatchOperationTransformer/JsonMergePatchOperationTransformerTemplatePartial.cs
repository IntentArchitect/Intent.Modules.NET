using System;
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

namespace Intent.Modules.AspNetCore.Controllers.JsonPatch.Templates.Templates.JsonMergePatchOperationTransformer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class JsonMergePatchOperationTransformerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Controllers.JsonPatch.Templates.JsonMergePatchOperationTransformer";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public JsonMergePatchOperationTransformerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.AspNetCore.OpenApi")
                .AddUsing("Microsoft.OpenApi")
                .AddUsing("Morcatko.AspNetCore.JsonMergePatch")
                .AddClass($"JsonMergePatchOperationTransformer", @class =>
                {
                    @class.ImplementsInterface("IOpenApiOperationTransformer");

                    @class.AddMethod("Task", "TransformAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("OpenApiOperation", "operation");
                        method.AddParameter("OpenApiOperationTransformerContext", "context");
                        method.AddParameter("CancellationToken", "cancellationToken");
                        method.AddStatements(
                            """
                                if (context.Description.HttpMethod != "PATCH")
                                {
                                    return;
                                }

                                if (operation.RequestBody?.Content == null)
                                {
                                    return;
                                }

                                var patchParam = context.Description.ParameterDescriptions
                                    .FirstOrDefault(param => param.Type.IsGenericType && param.Type.GetGenericTypeDefinition() == typeof(JsonMergePatchDocument<>));

                                if (patchParam == null)
                                {
                                    return;
                                }

                                var payloadType = patchParam.Type.GetGenericArguments()[0];

                                var payloadSchemaReference = await context.GetOrCreateSchemaAsync(payloadType, cancellationToken: cancellationToken);
                                if (payloadSchemaReference == null)
                                {
                                    return;
                                }

                                payloadSchemaReference.Required?.Remove("patchExecutor");
                                payloadSchemaReference.Properties?.Remove("patchExecutor");

                                if (operation.RequestBody.Content.TryGetValue(JsonMergePatchDocument.ContentType, out var content))
                                {
                                    content.Schema = payloadSchemaReference;
                                }
                                """.ConvertToStatements());
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() &&
                   ExecutionContext.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Distribution.ScalarConfiguration")).Any();
        }

        public override void AfterTemplateRegistration()
        {
            var templates =
                ExecutionContext.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Distribution.ScalarConfiguration"));

            foreach (var template in templates)
            {
                template.CSharpFile.OnBuild(file =>
                {
                    var cls = file.Classes.First();
                    var configureOpenApiOptionsBlock = GetConfigureOpenApiOptionsBlock(cls);
                    if (configureOpenApiOptionsBlock == null)
                    {
                        return;
                    }

                    configureOpenApiOptionsBlock.AddStatement($@"options.AddOperationTransformer(new {template.GetJsonMergePatchOperationTransformerName()}());");
                });
            }
        }
        
        private static CSharpLambdaBlock? GetConfigureOpenApiOptionsBlock(CSharpClass @class)
        {
            var configureOpenApiMethod = @class.FindMethod("ConfigureOpenApi");
            var addOpenApi = configureOpenApiMethod?.FindStatement(p => p.GetText("").Contains("AddOpenApi")) as CSharpInvocationStatement;
            var cSharpLambdaBlock = addOpenApi?.Statements.First() as CSharpLambdaBlock;
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