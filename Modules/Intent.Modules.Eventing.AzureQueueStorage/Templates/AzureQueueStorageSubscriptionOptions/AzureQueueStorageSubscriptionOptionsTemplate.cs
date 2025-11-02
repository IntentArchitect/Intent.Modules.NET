using Intent.Modules.Eventing.Contracts.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly:IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpStringInterpolationTemplate",Version= "1.0")]

namespace Intent.Modules.Eventing.AzureQueueStorage.Templates.AzureQueueStorageSubscriptionOptions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AzureQueueStorageSubscriptionOptionsTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return $$"""
                     using System;

                     [assembly: DefaultIntentManaged(Mode.Fully)]

                     namespace {{Namespace}}
                     {
                        
                        public class {{ClassName}}
                        {
                            private readonly {{UseType("System.Collections.Generic.List")}}<SubscriptionEntry> _entries = new List<SubscriptionEntry>();

                            public {{UseType("System.Collections.Generic.IReadOnlyList")}}<SubscriptionEntry> Entries => _entries;

                            public void Add<TMessage, THandler>(string queueName)
                                where TMessage : class
                                where THandler : {{this.GetIntegrationEventHandlerInterfaceName()}}<TMessage>
                            {
                                ArgumentNullException.ThrowIfNull(queueName);
                                _entries.Add(new SubscriptionEntry(typeof(TMessage), {{this.GetAzureQueueStorageEventDispatcherName()}}.InvokeDispatchHandler<TMessage, THandler>, queueName));
                            }
                        }

                        public delegate {{UseType("System.Threading.Tasks.Task")}} DispatchHandler(IServiceProvider serviceProvider, {{this.GetAzureQueueStorageEnvelopeName()}} message, {{UseType("System.Text.Json.JsonSerializerOptions")}} serializationOptions, {{UseType("System.Threading.CancellationToken")}} cancellationToken);

                        public record SubscriptionEntry(Type MessageType, DispatchHandler HandlerAsync, string QueueName);
                     }
                     """;
        }
    }
}