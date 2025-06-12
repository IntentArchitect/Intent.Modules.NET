using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly:IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpStringInterpolationTemplate",Version= "1.0")]

namespace Intent.Modules.Eventing.AzureEventGrid.Templates.AzureEventGridPipeline
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AzureEventGridPipelineTemplate
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

                     namespace {{Namespace}};
                     
                     public abstract class AzureEventGridPipelineBase<TBehavior>
                     {
                         private readonly List<TBehavior> _behaviors;
                     
                         protected AzureEventGridPipelineBase(IEnumerable<TBehavior> behaviors)
                         {
                             _behaviors = new List<TBehavior>(behaviors);
                         }
                     
                         public Task<CloudEvent> ExecuteAsync(CloudEvent message, Func<CloudEvent, CancellationToken, Task<CloudEvent>> finalHandler, CancellationToken cancellationToken = default)
                         {
                             var pipeline = BuildPipeline(finalHandler);
                             return pipeline(message, cancellationToken);
                         }
                     
                         private CloudEventBehaviorDelegate BuildPipeline(Func<CloudEvent, CancellationToken, Task<CloudEvent>> finalHandler)
                         {
                             CloudEventBehaviorDelegate pipeline = (message, token) => finalHandler(message, token);
                     
                             for (var i = _behaviors.Count - 1; i >= 0; i--)
                             {
                                 var behavior = _behaviors[i];
                                 var next = pipeline;
                                 pipeline = (message, cancellationToken) => ExecuteBehavior(behavior, message, next, cancellationToken);
                             }
                     
                             return pipeline;
                         }
                     
                         protected abstract Task<CloudEvent> ExecuteBehavior(TBehavior behavior, CloudEvent message, CloudEventBehaviorDelegate next, CancellationToken cancellationToken);
                     }
                     
                     public class {{AzureEventGridPublisherPipeline}} : AzureEventGridPipelineBase<{{this.GetAzureEventGridPublisherBehaviorInterfaceName()}}>
                     {
                         public {{AzureEventGridPublisherPipeline}}(IEnumerable<{{this.GetAzureEventGridPublisherBehaviorInterfaceName()}}> behaviors) : base(behaviors) { }
                     
                         protected override Task<CloudEvent> ExecuteBehavior({{this.GetAzureEventGridPublisherBehaviorInterfaceName()}} behavior, CloudEvent message, CloudEventBehaviorDelegate next, CancellationToken cancellationToken)
                             => behavior.HandleAsync(message, next, cancellationToken);
                     }
                     
                     public class {{AzureEventGridConsumerPipeline}} : AzureEventGridPipelineBase<{{this.GetAzureEventGridConsumerBehaviorInterfaceName()}}>
                     {
                         public {{AzureEventGridConsumerPipeline}}(IEnumerable<{{this.GetAzureEventGridConsumerBehaviorInterfaceName()}}> behaviors) : base(behaviors) { }
                     
                         protected override Task<CloudEvent> ExecuteBehavior({{this.GetAzureEventGridConsumerBehaviorInterfaceName()}} behavior, CloudEvent message, CloudEventBehaviorDelegate next, CancellationToken cancellationToken)
                             => behavior.HandleAsync(message, next, cancellationToken);
                     }
                     """;
        }
    }
}