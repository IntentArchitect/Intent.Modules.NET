using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.MongoDb.Finbuckle.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DependencyInjectionFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.MongoDb.Finbuckle.DependencyInjectionFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var dependencyInjection = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Infrastructure.DependencyInjection);
            if (dependencyInjection == null)
            {
                return;
            }

            dependencyInjection.CSharpFile.AfterBuild(file =>
            {
                file.Classes.First().FindMethod("AddInfrastructure")
                    .FindStatement(stmt => stmt.HasMetadata("application-mongodb-context"))
                    .Remove();
            });
        }
    }
}