using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly:IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpStringInterpolationTemplate",Version= "1.0")]

namespace Intent.Modules.Eventing.AzureServiceBus.Templates.AzureServiceBusPublisherOptions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AzureServiceBusPublisherOptionsTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return $$"""
                     using System;
                     using System.Collections.Generic;

                     [assembly: DefaultIntentManaged(Mode.Fully)]

                     namespace {{Namespace}}
                     {
                         public class {{ClassName}}
                         {
                             private readonly List<AzureServiceBusPublisherEntry> _entries = [];
                             
                             public IReadOnlyList<AzureServiceBusPublisherEntry> Entries => _entries;
                             
                             public void Add<TMessage>(string queueOrTopicName)
                             {
                                 ArgumentNullException.ThrowIfNull(queueOrTopicName);
                                 _entries.Add(new AzureServiceBusPublisherEntry(typeof(TMessage), queueOrTopicName));
                             }
                         }
                         
                         public record AzureServiceBusPublisherEntry(Type MessageType, string QueueOrTopicName);
                     }
                     """;
        }
    }
}