using System.Collections.Generic;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.AzureEventGrid.Templates.AzureEventGridBehavior;
using Intent.Modules.Eventing.AzureEventGrid.Templates.AzureEventGridConfiguration;
using Intent.Modules.Eventing.AzureEventGrid.Templates.AzureEventGridEventBus;
using Intent.Modules.Eventing.AzureEventGrid.Templates.AzureEventGridMessageDispatcher;
using Intent.Modules.Eventing.AzureEventGrid.Templates.AzureEventGridMessageDispatcherInterface;
using Intent.Modules.Eventing.AzureEventGrid.Templates.AzureEventGridPipeline;
using Intent.Modules.Eventing.AzureEventGrid.Templates.AzureEventGridPublisherOptions;
using Intent.Modules.Eventing.AzureEventGrid.Templates.AzureEventGridSubscriptionOptions;
using Intent.Modules.Eventing.AzureEventGrid.Templates.EventContext;
using Intent.Modules.Eventing.AzureEventGrid.Templates.EventContextInterface;
using Intent.Modules.Eventing.AzureEventGrid.Templates.InboundCloudEventBehavior;
using Intent.Modules.Eventing.AzureEventGrid.Templates.IntegrationEventHandler;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureEventGrid.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAzureEventGridBehaviorName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureEventGridBehaviorTemplate.TemplateId);
        }

        [IntentIgnore]
        public static string GetAzureEventGridConsumerBehaviorInterfaceName<TModel>(this Intent.Modules.Common.CSharp.Templates.CSharpTemplateBase<TModel> template)
        {
            return template.TryGetTemplate<Intent.Modules.Common.CSharp.Templates.ICSharpTemplate>(AzureEventGridBehaviorTemplate.TemplateId, out var t)
                ? template.NormalizeNamespace($"{t.Namespace}.{AzureEventGridBehaviorTemplate.IAzureEventGridConsumerBehavior}")
                : throw new System.InvalidOperationException();
        }

        [IntentIgnore]
        public static string GetAzureEventGridPublisherBehaviorInterfaceName<TModel>(this Intent.Modules.Common.CSharp.Templates.CSharpTemplateBase<TModel> template)
        {
            return template.TryGetTemplate<Intent.Modules.Common.CSharp.Templates.ICSharpTemplate>(AzureEventGridBehaviorTemplate.TemplateId, out var t)
                ? template.NormalizeNamespace($"{t.Namespace}.{AzureEventGridBehaviorTemplate.IAzureEventGridPublisherBehavior}")
                : throw new System.InvalidOperationException();
        }

        public static string GetAzureEventGridConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureEventGridConfigurationTemplate.TemplateId);
        }

        public static string GetAzureEventGridEventBusName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureEventGridEventBusTemplate.TemplateId);
        }

        public static string GetAzureEventGridMessageDispatcherName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureEventGridMessageDispatcherTemplate.TemplateId);
        }

        public static string GetAzureEventGridMessageDispatcherInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureEventGridMessageDispatcherInterfaceTemplate.TemplateId);
        }

        public static string GetAzureEventGridPipelineName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureEventGridPipelineTemplate.TemplateId);
        }

        [IntentIgnore]
        public static string GetAzureEventGridPublisherPipelineName<TModel>(this Intent.Modules.Common.CSharp.Templates.CSharpTemplateBase<TModel> template)
        {
            return template.TryGetTemplate<Intent.Modules.Common.CSharp.Templates.ICSharpTemplate>(AzureEventGridPipelineTemplate.TemplateId, out var t)
                ? template.NormalizeNamespace($"{t.Namespace}.{AzureEventGridPipelineTemplate.AzureEventGridPublisherPipeline}")
                : throw new System.InvalidOperationException();
        }

        [IntentIgnore]
        public static string GetAzureEventGridConsumerPipelineName<TModel>(this Intent.Modules.Common.CSharp.Templates.CSharpTemplateBase<TModel> template)
        {
            return template.TryGetTemplate<Intent.Modules.Common.CSharp.Templates.ICSharpTemplate>(AzureEventGridPipelineTemplate.TemplateId, out var t)
                ? template.NormalizeNamespace($"{t.Namespace}.{AzureEventGridPipelineTemplate.AzureEventGridConsumerPipeline}")
                : throw new System.InvalidOperationException();
        }

        public static string GetAzureEventGridPublisherOptionsName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureEventGridPublisherOptionsTemplate.TemplateId);
        }

        public static string GetAzureEventGridSubscriptionOptionsName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureEventGridSubscriptionOptionsTemplate.TemplateId);
        }

        public static string GetEventContextName(this IIntentTemplate template)
        {
            return template.GetTypeName(EventContextTemplate.TemplateId);
        }

        public static string GetEventContextInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(EventContextInterfaceTemplate.TemplateId);
        }

        public static string GetInboundCloudEventBehaviorName(this IIntentTemplate template)
        {
            return template.GetTypeName(InboundCloudEventBehaviorTemplate.TemplateId);
        }

        public static string GetIntegrationEventHandlerName<T>(this IIntentTemplate<T> template) where T : IntegrationEventHandlerModel
        {
            return template.GetTypeName(IntegrationEventHandlerTemplate.TemplateId, template.Model);
        }

        public static string GetIntegrationEventHandlerName(this IIntentTemplate template, IntegrationEventHandlerModel model)
        {
            return template.GetTypeName(IntegrationEventHandlerTemplate.TemplateId, model);
        }

    }
}