using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.ODataQuery.Templates.ODataQuerySwaggerFilter;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.ODataQuery.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SwaggerConfigExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.ODataQuery.SwaggerConfigExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            // An application can have several hosts, each with its own Swagger configuration.
            foreach (var template in application.FindTemplateInstances<ICSharpFileBuilderTemplate>("Distribution.SwashbuckleConfiguration"))
            {
                var @class = template.CSharpFile.Classes.First();

                var configureSwaggerOptionsBlock = GetConfigureSwaggerOptionsBlock(@class);
                if (configureSwaggerOptionsBlock == null)
                {
                    continue;
                }

                // Each host references the filter in its own project, placed by the filter's Template Output.
                if (template.OutputTarget.FindTemplateInstance(ODataQuerySwaggerFilterTemplate.TemplateId) is not { } filterTemplate ||
                    !filterTemplate.CanRunTemplate())
                {
                    Logging.Log.Warning($"OData Query: project '{template.OutputTarget.GetProject().Name}' configures Swagger but has no OData query filter it can reference, " +
                        "so its OpenAPI document will not describe the OData query options. " +
                        $"To describe them, add a Template Output for {ODataQuerySwaggerFilterTemplate.TemplateId} under that project in the Codebase Structure designer.");
                    continue;
                }

                configureSwaggerOptionsBlock.AddStatement($@"options.OperationFilter<{template.GetTypeName(filterTemplate)}>();");
            }
        }
        private static CSharpLambdaBlock GetConfigureSwaggerOptionsBlock(CSharpClass @class)
        {
            var configureSwaggerMethod = @class.FindMethod("ConfigureSwagger");
            var addSwaggerGen = configureSwaggerMethod?.FindStatement(p => p.HasMetadata("AddSwaggerGen")) as CSharpInvocationStatement;
            var cSharpLambdaBlock = addSwaggerGen?.Statements.First() as CSharpLambdaBlock;
            return cSharpLambdaBlock;
        }

    }
}
