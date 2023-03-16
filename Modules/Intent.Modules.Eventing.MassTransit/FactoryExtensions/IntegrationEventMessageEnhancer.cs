using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.FactoryExtensions;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public class IntegrationEventMessageEnhancer : FactoryExtensionBase
{
    public override string Id => "Intent.Eventing.MassTransit.IntegrationEventMessageEnhancer";

    [IntentManaged(Mode.Ignore)] public override int Order => 0;

    protected override void OnAfterTemplateRegistrations(IApplication application)
    {
        var templates = application.FindTemplateInstances<IntegrationEventMessageTemplate>(TemplateDependency.OnTemplate(IntegrationEventMessageTemplate.TemplateId));
        foreach (var template in templates)
        {
            template.CSharpFile.AfterBuild(file =>
            {
                var rec = file.Records.First();
                rec.Properties.Insert(0, new CSharpProperty(template.UseType("System.Guid") + "?", "CorrelationId", rec).Init());
            }, 1000);
        }
    }
}