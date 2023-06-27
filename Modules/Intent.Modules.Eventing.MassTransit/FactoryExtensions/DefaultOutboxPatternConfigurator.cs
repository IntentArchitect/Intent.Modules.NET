using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.MassTransit.Settings;
using Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DefaultOutboxPatternConfigurator : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.MassTransit.DefaultOutboxPatternConfigurator";

        [IntentManaged(Mode.Ignore)] public override int Order => 0;
        
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            if (application.Settings.GetEventingSettings().OutboxPattern().IsNone())
            {
                return;
            }
            
            var template = application.FindTemplateInstance<MassTransitConfigurationTemplate>(TemplateDependency.OnTemplate(MassTransitConfigurationTemplate.TemplateId));
            if (template == null)
            {
                return;
            }
            
            if (application.Settings.GetEventingSettings().OutboxPattern().IsInMemory())
            {
                //template.MessageProviderSpecificConfigCode.NestedConfigurationCodeLines.Add("cfg.UseInMemoryOutbox();");
                template.CSharpFile.OnBuild(file =>
                {
                    var priClass = file.Classes.First();
                    var method = priClass.FindMethod("AddMassTransitConfiguration");
                    if (method.FindStatement(p => p.HasMetadata("configure-masstransit")) is not CSharpInvocationStatement configMassTransit) { return; }
                    if (configMassTransit.FindStatement(p => p.HasMetadata("message-broker")) is not CSharpInvocationStatement messageBrokerInv) { return; }
                    if (messageBrokerInv.Statements.First() is not CSharpLambdaBlock messageBrokerInvBlock) { return; }

                    messageBrokerInvBlock.AddStatement($@"cfg.UseInMemoryOutbox();");
                });

                return;
            }
            
            if (application.Settings.GetEventingSettings().OutboxPattern().IsEntityFramework()
                && application.GetApplicationConfig().Modules.All(p => p.ModuleId != "Intent.Eventing.MassTransit.EntityFrameworkCore"))
            {
                Logging.Log.Warning("Please install Intent.Eventing.MassTransit.EntityFrameworkCore module for the Outbox pattern to persist to the database");
            }
        }
    }
}