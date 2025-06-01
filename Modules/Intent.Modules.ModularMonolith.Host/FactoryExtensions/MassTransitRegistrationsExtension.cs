using System.Linq;
using System.Xml.Schema;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.ModularMonolith.Host.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.ModularMonolith.Host.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MassTransitRegistrationsExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.ModularMonolith.Host.MassTransitRegistrationsExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            base.OnAfterTemplateRegistrations(application);
            var massTransitModuleConfig = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Eventing.MassTransit.MassTransitConfiguration");

            massTransitModuleConfig?.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                var addMassTransitConfigurationMethod = @class.FindMethod("AddMassTransitConfiguration");
                var massTransitConfig = (IHasCSharpStatements)addMassTransitConfigurationMethod.FindStatement(p => p.HasMetadata("configure-masstransit"));
                var massTransitConfigLambda = (IHasCSharpStatements)massTransitConfig.Statements.First();

                addMassTransitConfigurationMethod.AddParameter($"IEnumerable<{massTransitModuleConfig.GetModuleInstallerInterfaceName()}>", "moduleInstallers");
                massTransitConfigLambda.AddStatement("moduleInstallers.ConfigureIntegrationEventConsumers(x);");
            }, 1);

        }
    }
}