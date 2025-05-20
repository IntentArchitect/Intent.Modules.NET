using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.ModularMonolith.Host.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.ModularMonolith.Host.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SwaggerConfigExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.ModularMonolith.Host.SwaggerConfigExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var swaggerConfig = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.AspNetCore.Swashbuckle.SwashbuckleConfiguration");

            swaggerConfig?.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("System.Collections.Generic");
                var @class = file.Classes.First();

                var configureSwaggerMethod = @class.FindMethod("ConfigureSwagger");
                var addSwaggerGen = configureSwaggerMethod?.FindStatement(p => p.HasMetadata("AddSwaggerGen")) as CSharpInvocationStatement;
                var configureSwaggerOptionsBlock = addSwaggerGen?.Statements.First() as CSharpLambdaBlock;

                if (configureSwaggerOptionsBlock == null)
                {
                    return;
                }
                configureSwaggerMethod.AddParameter($"IEnumerable<{swaggerConfig.GetModuleInstallerInterfaceName()}>", "moduleInstallers");
                configureSwaggerOptionsBlock.AddStatement($@"moduleInstallers.ConfigureSwagger(options);");
            });
        }
    }
}