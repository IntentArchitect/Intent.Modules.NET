using System.Linq;
using Intent.Engine;
using Intent.Eventing.Contracts.DomainMapping.Api;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.DomainMapping.Templates;
using Intent.Modules.Eventing.Contracts.DomainMapping.Templates.MessageExtensions;
using Intent.Modules.Eventing.Contracts.Templates.EventBusInterface;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.Contracts.DomainMapping.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DomainEventHandlerMessagePublishingImplementationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.Contracts.DomainMapping.DomainEventHandlerMessagePublishingImplementationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            foreach (var messageModel in application.MetadataManager.Eventing(application).GetMessageModels().Where(x => x.HasMapFromDomainMapping() && x.GetMapFromDomainMapping().Element.IsDomainEventModel()))
            {
                var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Application.DomainEventHandler.OldDefault, messageModel.GetMapFromDomainMapping().ElementId)
                    ?? application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Application.DomainEventHandler.Default, messageModel.GetMapFromDomainMapping().ElementId);
                if (template != null)
                {
                    template.CSharpFile.OnBuild(file =>
                    {
                        var extensionExists = template.TryGetTypeName(MessageExtensionsTemplate.TemplateId, messageModel, out _); // adds using
                        if (extensionExists)
                        {
                            var @class = file.Classes.First();
                            @class.Constructors.First()
                                .AddParameter(template.GetTypeName(EventBusInterfaceTemplate.TemplateId), "eventBus", param => param.IntroduceReadonlyField());
                            @class.FindMethod("Handle")
                                .AddStatement($"var integrationEvent = notification.DomainEvent.MapTo{messageModel.Name.ToPascalCase()}();")
                                .AddStatement("_eventBus.Publish(integrationEvent);");
                        }
                    });
                }
            }
        }
    }
}