using System.Linq;
using System.Threading;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Eventing.Solace.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.Solace.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class BaseMessageExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.Solace.BaseMessageExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {

            //Events
            var serviceDesignerSubEvents = application.MetadataManager
                .Services(application.GetApplicationConfig().Id).GetIntegrationEventHandlerModels()
                .SelectMany(x => x.IntegrationEventSubscriptions())
                .Select(x => x.TypeReference.Element.AsMessageModel());

            var eventingDesignerSubEvents = application.MetadataManager
                .Eventing(application.GetApplicationConfig().Id).GetApplicationModels()
                .SelectMany(x => x.SubscribedMessages())
                .Select(x => x.TypeReference.Element.AsMessageModel());

            var messages = Enumerable.Empty<MessageModel>()
                .Concat(serviceDesignerSubEvents)
                .Concat(eventingDesignerSubEvents)
                .OrderBy(x => x.Name)
                .ToArray();

            var serviceDesignerPubEvents = application.MetadataManager
                .GetExplicitlyPublishedMessageModels(application);

            var eventingDesignerPubEvents = application.MetadataManager
                .Eventing(application.GetApplicationConfig().Id).GetApplicationModels()
                .SelectMany(x => x.PublishedMessages())
                .Select(x => x.TypeReference.Element.AsMessageModel());

            messages = messages
                    .Concat(serviceDesignerPubEvents)
                    .Concat(eventingDesignerPubEvents)
                    .OrderBy(x => x.Name)
                    .ToArray();

            //Commands
            var serviceDesignerSubCommands = application.MetadataManager
                .Services(application.GetApplicationConfig().Id).GetIntegrationEventHandlerModels()
                .SelectMany(x => x.IntegrationCommandSubscriptions())
                .Select(x => x.TypeReference.Element.AsIntegrationCommandModel());

            var commands = Enumerable.Empty<IntegrationCommandModel>()
                .Concat(serviceDesignerSubCommands)
                .OrderBy(x => x.Name)
                .ToArray();

            var serviceDesignerPubCommands = application.MetadataManager
                .GetExplicitlySentIntegrationCommandDispatches(application.GetApplicationConfig().Id)
                .Select(x => x.TypeReference.Element.AsIntegrationCommandModel());

            commands = commands
                        .Concat(serviceDesignerPubCommands)
                        .OrderBy(x => x.Name)
                        .ToArray();

            foreach (var message in messages)
            {
                var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Eventing.Contracts.IntegrationEventMessage", message);
                if (template == null)
                {
                    return;
                }
                template.CSharpFile.OnBuild(file =>
                {
                    var @class = file.TypeDeclarations.FirstOrDefault();
                    @class.WithBaseType(template.GetBaseMessageName());
                });
            }

            foreach (var command in commands)
            {
                var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Eventing.Contracts.IntegrationCommand", command);
                if (template == null)
                {
                    return;
                }
                template.CSharpFile.OnBuild(file =>
                {
                    var @class = file.TypeDeclarations.FirstOrDefault();
                    @class.WithBaseType(template.GetBaseMessageName());
                });
            }
        }
    }
}