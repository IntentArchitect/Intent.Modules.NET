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

namespace Intent.Modules.HotChocolate.GraphQL.AzureFunctions.Templates.GraphQLFunction
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class GraphQLFunctionTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.HotChocolate.GraphQL.AzureFunctions.GraphQLFunctionTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public GraphQLFunctionTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.HotChocolateAzureFunctions(OutputTarget));
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading.Tasks")
                .AddClass($"GraphQLFunction", @class =>
                {
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(UseType("HotChocolate.AzureFunctions.IGraphQLRequestExecutor"), "executor", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });
                    @class.AddMethod($"Task<{UseType("Microsoft.AspNetCore.Mvc.IActionResult")}>", "Run", method =>
                    {
                        method.AddAttribute(UseType("Microsoft.Azure.WebJobs.FunctionName"), attr => attr.AddArgument("\"GraphQLHttpFunction\""));

                        method.AddParameter(UseType("Microsoft.AspNetCore.Http.HttpRequest"), "request", param =>
                        {
                            param.AddAttribute("HttpTrigger", attr =>
                            {
                                //This might need to be configurable
                                attr.AddArgument($"{UseType("Microsoft.Azure.WebJobs.Extensions.Http.AuthorizationLevel")}.Anonymous");
                                attr.AddArgument($"\"get\"");
                                attr.AddArgument($"\"post\"");
                                attr.AddArgument($"Route = \"graphql/{{**slug}}\"");
                            });
                        });

                        method.AddStatement("return _executor.ExecuteAsync(request);");
                    });
                });
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