using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly:IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpStringInterpolationTemplate",Version= "1.0")]

namespace Intent.Modules.Eventing.AzureEventGrid.Templates.AzureEventGridBehavior
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AzureEventGridBehaviorTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return $$"""
                     using System;
                     using System.Threading;
                     using System.Threading.Tasks;
                     using Azure.Messaging;

                     [assembly: DefaultIntentManaged(Mode.Fully)]

                     namespace {{Namespace}};
                     
                     public interface {{IAzureEventGridPublisherBehavior}}
                     {
                         Task<CloudEvent> HandleAsync(CloudEvent cloudEvent, CloudEventBehaviorDelegate next, CancellationToken cancellationToken = default);
                     }
                     
                     public interface {{IAzureEventGridConsumerBehavior}}
                     {
                         Task<CloudEvent> HandleAsync(CloudEvent cloudEvent, CloudEventBehaviorDelegate next, CancellationToken cancellationToken = default);
                     }
                     
                     public delegate Task<CloudEvent> CloudEventBehaviorDelegate(CloudEvent cloudEvent, CancellationToken cancellationToken);
                     """;
        }
    }
}