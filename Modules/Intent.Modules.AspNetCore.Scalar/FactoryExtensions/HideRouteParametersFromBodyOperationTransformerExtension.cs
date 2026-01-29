using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.Scalar.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Scalar.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class HideRouteParametersFromBodyOperationTransformerExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Scalar.HideRouteParametersFromBodyOperationTransformerExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Intent.AspNetCore.Scalar.OpenApiConfiguration"));

            foreach (var template in templates)
            {
                if (template == null)
                {
                    return;
                }

                var @class = template.CSharpFile.Classes.First();

                var configureOpenApiOptionsBlock = GetConfigureOpenApiOptionsBlock(@class);
                if (configureOpenApiOptionsBlock == null)
                {
                    return;
                }

                configureOpenApiOptionsBlock.AddStatement($@"options.AddOperationTransformer(new {template.GetHideRouteParametersFromBodyOperationTransformerName()}());", stmt => stmt.SeparatedFromPrevious());
            }
        }

        private static CSharpLambdaBlock GetConfigureOpenApiOptionsBlock(CSharpClass @class)
        {
            var configureOpenApiMethod = @class.FindMethod("ConfigureOpenApi");
            var addOpenApi = configureOpenApiMethod?.FindStatement(s => s.ToString().Contains("services.AddOpenApi")) as CSharpInvocationStatement;
            var cSharpLambdaBlock = addOpenApi?.Statements.FirstOrDefault() as CSharpLambdaBlock;
            return cSharpLambdaBlock;
        }
    }
}
