using Intent.Modules.Eventing.Contracts.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly:IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpStringInterpolationTemplate",Version= "1.0")]

namespace Intent.Modules.Eventing.AzureEventGrid.Templates.AzureEventGridSubscriptionOptions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AzureEventGridSubscriptionOptionsTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return $$"""
                     using System;
                     using System.Collections.Generic;
                     using System.Threading;
                     using System.Threading.Tasks;
                     using Azure.Messaging;

                     [assembly: DefaultIntentManaged(Mode.Fully)]

                     namespace {{Namespace}}
                     {
                         public class {{ClassName}}
                         {
                             private readonly List<SubscriptionEntry> _entries = [];
                             
                             public IReadOnlyList<SubscriptionEntry> Entries => _entries;
                             
                             public void Add<TMessage, THandler>()
                                 where TMessage : class
                                 where THandler : {{this.GetIntegrationEventHandlerInterfaceName()}}<TMessage>
                             {
                                 _entries.Add(new SubscriptionEntry(typeof(TMessage), {{this.GetAzureEventGridMessageDispatcherName()}}.InvokeDispatchHandler<TMessage, THandler>));
                             }
                         }
                         
                         public delegate Task DispatchHandler(IServiceProvider serviceProvider, CloudEvent cloudEvent, CancellationToken cancellationToken);
                         
                         public record SubscriptionEntry(Type MessageType, DispatchHandler HandlerAsync);
                     }
                     """;
        }
    }
}