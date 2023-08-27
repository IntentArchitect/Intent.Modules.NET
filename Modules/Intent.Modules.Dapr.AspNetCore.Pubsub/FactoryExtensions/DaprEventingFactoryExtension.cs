using System.Linq;
using System.Reflection;
using Intent.Dapr.AspNetCore.Pubsub.Api;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapr.AspNetCore.Pubsub.Templates;
using Intent.Modules.Eventing.Contracts.Templates.EventBusInterface;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.Pubsub.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DaprEventingFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Dapr.AspNetCore.Pubsub.DaprEventingFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            var eventBusInterfaceTemplate = application.FindTemplateInstance<EventBusInterfaceTemplate>(EventBusInterfaceTemplate.TemplateId);
            eventBusInterfaceTemplate.CSharpFile.AfterBuild(file =>
            {
                var @interface = file.Interfaces.Single();
                var method = @interface.Methods.Single(x => x.Name == "Publish");
                var constraint = method.GenericTypeConstraints.Single(x => x.GenericTypeParameter == "T");
                constraint.AddType(eventBusInterfaceTemplate.GetEventInterfaceName());
            });

            var eventTemplates = application.FindTemplateInstances<IntegrationEventMessageTemplate>(TemplateDependency.OfType<IntegrationEventMessageTemplate>());
            foreach (var template in eventTemplates)
            {
                template.CSharpFile.AfterBuild(file =>
                {
                    var interfaceType = template.GetEventInterfaceName();
                    file.Records.Single()
                        .ImplementsInterface(interfaceType)
                        .AddField("string", "PubsubName", f => f.Constant(PubsubName(template)))
                        .AddField("string", "TopicName", f => f.Constant(TopicName(template)))
                        .AddProperty("string", "PubsubName", p => p
                            .ExplicitlyImplements(interfaceType)
                            .WithoutSetter()
                            .Getter.WithExpressionImplementation("PubsubName")
                        )
                        .AddProperty("string", "TopicName", p => p
                            .ExplicitlyImplements(interfaceType)
                            .WithoutSetter()
                            .Getter.WithExpressionImplementation("TopicName")
                        )
                        ;
                });
            }
        }

        private static string PubsubName(IntegrationEventMessageTemplate template)
        {
            var pubsub = template.Model.GetDaprSettings()?.PubsubName();

            return !string.IsNullOrWhiteSpace(pubsub)
                ? $"\"{pubsub}\""
                : $"\"pubsub\"";
        }

        private static string TopicName(IntegrationEventMessageTemplate template)
        {
            var topic = template.Model.GetDaprSettings()?.TopicName();

            return !string.IsNullOrWhiteSpace(topic)
                ? $"\"{topic}\""
                : $"nameof({template.ClassName})";
        }
    }
}