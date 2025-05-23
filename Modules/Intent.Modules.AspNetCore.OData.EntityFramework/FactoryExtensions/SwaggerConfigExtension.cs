using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.OData.EntityFramework.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.OData.EntityFramework.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SwaggerConfigExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.OData.EntityFramework.SwaggerConfigExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Distribution.SwashbuckleConfiguration"));
            if (template == null)
            {
                return;
            }

            template.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                var configureSwaggerOptionsBlock = GetConfigureSwaggerOptionsBlock(@class);
                file.AddUsing("System.Linq");
                if (configureSwaggerOptionsBlock is null)
                {
                    return;
                }

                configureSwaggerOptionsBlock.AddStatement($@"options.DocumentFilter<{template.GetExcludeODataDocumentFilterName()}>();");
                configureSwaggerOptionsBlock.AddStatement("options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());");
            });
        }

        private static CSharpLambdaBlock? GetConfigureSwaggerOptionsBlock(CSharpClass @class)
        {
            var configureSwaggerMethod = @class.FindMethod("ConfigureSwagger");
            var addSwaggerGen = configureSwaggerMethod?.FindStatement(p => p.HasMetadata("AddSwaggerGen")) as CSharpInvocationStatement;
            var cSharpLambdaBlock = addSwaggerGen?.Statements.First() as CSharpLambdaBlock;
            return cSharpLambdaBlock;
        }
    }
}