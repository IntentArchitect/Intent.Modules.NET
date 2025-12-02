using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Dapr.AspNetCore.Pubsub.Api;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationCommand;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.DaprConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DaprConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Dapr.AspNetCore.Pubsub.DaprConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DaprConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            FulfillsRole(TemplateRoles.Application.Eventing.MessageBusConfiguration);
            
            var publishedMessages = GetPublishedMessages();

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass("DaprConfiguration", @class =>
                {
                    @class.Static();

                    @class.AddMethod("IServiceCollection", "AddDaprConfiguration", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", param => param.WithThisModifier());
                        method.AddParameter("IConfiguration", "configuration");

                        var requiresCompositeMessageBus = this.RequiresCompositeMessageBus();
                        if (requiresCompositeMessageBus)
                        {
                            method.AddParameter(this.GetMessageBrokerRegistryName(), "registry");
                            method.AddStatement($"services.AddScoped<{this.GetDaprMessageBusName()}>();", s => s.SeparatedFromNext());
                        }
                        else
                        {
                            var busInterface = this.GetBusInterfaceName();
                            method.AddStatement($"services.AddScoped<{this.GetDaprMessageBusName()}>();", s => s.SeparatedFromNext());
                            method.AddStatement($"services.AddScoped<{busInterface}, {this.GetDaprMessageBusName()}>();", s => s.SeparatedFromNext());
                        }

                        if (requiresCompositeMessageBus && publishedMessages.Any())
                        {
                            foreach (var message in publishedMessages)
                            {
                                method.AddStatement($"registry.Register<{GetMessageTypeName(message)}, {this.GetDaprMessageBusName()}>();");
                            }
                        }

                        method.AddReturn("services");
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            if (!this.RequiresCompositeMessageBus())
            {
                ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                    .ToRegister("AddDaprConfiguration", ServiceConfigurationRequest.ParameterType.Configuration)
                    .HasDependency(this));
            }
        }

        public override bool CanRunTemplate()
        {
            return GetPublishedMessages().Any();
        }

        private List<IElement> GetPublishedMessages()
        {
            var services = ExecutionContext.MetadataManager.Services(ExecutionContext.GetApplicationConfig().Id);

            var messages = services.GetMessageModels().SelectMany(x => x.IntegrationEventsSources())
                .Select(x => x.InternalElement)
                .Where(x => x.IsMessageModel())
                .FilterMessagesForThisMessageBroker(this, [MessageModelStereotypeExtensions.DaprSettings.DefinitionId])
                .ToList();

            return messages;
        }

        private string GetMessageTypeName(IElement message)
        {
            return true switch
            {
                _ when message.IsMessageModel() => GetTypeName(IntegrationEventMessageTemplate.TemplateId, message),
                _ when message.IsIntegrationCommandModel() => GetTypeName(IntegrationCommandTemplate.TemplateId, message),
                _ => throw new NotSupportedException($"Message type '{message.GetType().Name}' is not supported.")
            };
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}
