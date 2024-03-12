using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.Kafka.Templates.KafkaConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class KafkaConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Kafka.KafkaConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public KafkaConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Confluent.SchemaRegistry")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"KafkaConfiguration", @class =>
                {
                    @class.Static();

                    @class.AddMethod("void", "AddKafkaConfiguration", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", p => p.WithThisModifier());

                        method.AddInvocationStatement("services.AddSingleton<ISchemaRegistryClient>", invocation =>
                        {
                            invocation.AddArgument(new CSharpLambdaBlock("serviceProvider"), statement =>
                            {
                                var block = statement as CSharpLambdaBlock;

                                block.AddStatement(@"var schemaRegistryConfig = serviceProvider
                    .GetRequiredService<IConfiguration>()
                    .GetSection(""Kafka:SchemaRegistryConfig"")
                    .Get<SchemaRegistryConfig>();");

                                block.AddStatement("return new CachedSchemaRegistryClient(schemaRegistryConfig);", s => s.SeparatedFromPrevious());
                            });
                        });
                        method.AddStatement($"services.AddScoped<{this.GetEventBusInterfaceName()}, {this.GetKafkaEventBusName()}>();");
                        method.AddStatement($"services.AddHostedService<{this.GetKafkaConsumerBackgroundServiceName()}>();");

                        var messageTypeNames = GetMessageTypeNames();
                        foreach (var messageTypeName in messageTypeNames)
                        {
                            method.AddStatement($"services.AddTransient<{this.GetKafkaConsumerInterfaceName()}, {this.GetKafkaConsumerName()}<{messageTypeName}>>();");
                        }
                    });
                });
        }

        private IEnumerable<string> GetMessageTypeNames()
        {
            var serviceDesignerMessages = ExecutionContext.MetadataManager
                .Services(ExecutionContext.GetApplicationConfig().Id).GetIntegrationEventHandlerModels()
                .SelectMany(x => x.IntegrationEventSubscriptions())
                .Select(x => x.TypeReference.Element.AsMessageModel());

            var eventingDesignerMessages = ExecutionContext.MetadataManager
                .Eventing(ExecutionContext.GetApplicationConfig().Id).GetApplicationModels()
                .SelectMany(x => x.SubscribedMessages())
                .Select(x => x.TypeReference.Element.AsMessageModel());

            var messageTypeNames = Enumerable.Empty<MessageModel>()
                .Concat(serviceDesignerMessages)
                .Concat(eventingDesignerMessages)
                .Select(this.GetIntegrationEventMessageName);

            return messageTypeNames;
        }

        public override void AfterTemplateRegistration()
        {
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("AddKafkaConfiguration")
                .ForConcern("Infrastructure")
                .HasDependency(this));

            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("Kafka:SchemaRegistryConfig",
                new
                {
                    Url = "",
                    BasicAuthUserInfo = "",
                }));
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