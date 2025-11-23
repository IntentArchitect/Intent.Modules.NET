using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureServiceBus.Templates.AzureServiceBusMessageDispatcher
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AzureServiceBusMessageDispatcherTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.AzureServiceBus.AzureServiceBusMessageDispatcher";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureServiceBusMessageDispatcherTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.AzureMessagingServiceBus(OutputTarget));
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Text.Json")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Azure.Messaging.ServiceBus")
                .AddUsing("Microsoft.Extensions.Options")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"AzureServiceBusMessageDispatcher", @class =>
                {
                    @class.AddField("Dictionary<string, AzureServiceBusDispatchHandler>", "_handlers", field => field.PrivateReadOnly());

                    @class.ImplementsInterface(this.GetAzureServiceBusMessageDispatcherInterfaceName());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"IOptions<{this.GetAzureServiceBusSubscriptionOptionsName()}>", "options");

                        ctor.AddStatement("_handlers = options.Value.Entries.ToDictionary(k => k.MessageType.FullName!, v => v.HandlerAsync);");
                    });

                    @class.AddMethod("async Task", "DispatchAsync", method =>
                    {
                        method.AddParameter("IServiceProvider", "scopedServiceProvider");
                        method.AddParameter("ServiceBusReceivedMessage", "message");
                        method.AddParameter("CancellationToken", "cancellationToken");

                        method.AddStatement("""var messageTypeName = message.ApplicationProperties["MessageType"].ToString()!;""");
                        method.AddIfStatement("_handlers.TryGetValue(messageTypeName, out var handlerAsync)",
                            block => { block.AddStatement("await handlerAsync(scopedServiceProvider, message, cancellationToken);"); });
                    });

                    @class.AddMethod("Task", "InvokeDispatchHandler", method =>
                    {
                        method.Async().Static();
                        method.AddGenericParameter("TMessage", out var tMessage);
                        method.AddGenericParameter("THandler", out var tHandler);
                        method.AddParameter("IServiceProvider", "serviceProvider");
                        method.AddParameter("ServiceBusReceivedMessage", "message");
                        method.AddParameter("CancellationToken", "cancellationToken");
                        method.AddGenericTypeConstraint(tMessage, c => c.AddType("class"));
                        method.AddGenericTypeConstraint(tHandler, c => c.AddType($"{this.GetIntegrationEventHandlerInterfaceName()}<{tMessage}>"));

                        method.AddStatement($"var messageObj = (await JsonSerializer.DeserializeAsync<{tMessage}>(message.Body.ToStream(), cancellationToken: cancellationToken))!;");
                        method.AddStatement($"var handler = serviceProvider.GetRequiredService<{tHandler}>();");
                        method.AddStatement("await handler.HandleAsync(messageObj, cancellationToken);");
                    });
                });
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