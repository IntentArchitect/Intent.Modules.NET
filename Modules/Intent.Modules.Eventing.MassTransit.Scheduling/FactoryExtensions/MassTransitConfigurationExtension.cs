using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Scheduling.FactoryExtensions;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public class MassTransitConfigurationExtension : FactoryExtensionBase
{
    public override string Id => "Intent.Eventing.MassTransit.Scheduling.MassTransitConfigurationExtension";

    [IntentManaged(Mode.Ignore)]
    public override int Order => 0;

    protected override void OnAfterTemplateRegistrations(IApplication application)
    {
        var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Infrastructure.DependencyInjection.MassTransit"));
        template?.CSharpFile.OnBuild(file =>
        {
            var priClass = file.Classes.First();
            var massTransitConfig = (IHasCSharpStatements)priClass.FindMethod("AddMassTransitConfiguration")
                .FindStatement(p => p.HasMetadata("configure-masstransit"));
            var massTransitConfigLambda = (IHasCSharpStatements)massTransitConfig.Statements.First();
            massTransitConfigLambda.AddStatement("x.AddDelayedMessageScheduler();");

            var messageBrokerConfig = (IHasCSharpStatements)massTransitConfigLambda.FindStatement(p => p.HasMetadata("message-broker"));
            var messageBrokerConfigLambda = (IHasCSharpStatements)messageBrokerConfig.Statements.First();
            messageBrokerConfigLambda.AddStatement("cfg.UseDelayedMessageScheduler();");
        });
    }
}