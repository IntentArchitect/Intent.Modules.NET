using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Settings;
using Intent.Modules.Eventing.Contracts.Templates.CompositeMessageBusConfiguration;
using Intent.Modules.Eventing.Contracts.Templates.EventBusInterface;
using Intent.Modules.Eventing.Contracts.Templates.MessageBusInterface;

namespace Intent.Modules.Eventing.Contracts.Templates;

public static class MessageBusExtensions
{
    /// <summary>
    /// Returns the appropriate bus interface name based on the UseLegacyInterfaceName setting.
    /// If true, returns IEventBus; if false, returns IMessageBus.
    /// </summary>
    public static string GetBusInterfaceName(this IIntentTemplate template)
    {
        var useLegacy = template.ExecutionContext.Settings.GetEventingSettings().UseLegacyInterfaceName();
        return useLegacy
            ? template.GetTypeName(EventBusInterfaceTemplate.TemplateId)
            : template.GetTypeName(MessageBusInterfaceTemplate.TemplateId);
    }

    /// <summary>
    /// Returns the appropriate bus interface template ID based on the UseLegacyInterfaceName setting.
    /// If true, returns EventBusInterface template ID; if false, returns MessageBusInterface template ID.
    /// </summary>
    public static string GetBusInterfaceTemplateId(this ISoftwareFactoryExecutionContext factoryExecutionContext)
    {
        var useLegacy = factoryExecutionContext.Settings.GetEventingSettings().UseLegacyInterfaceName();
        return useLegacy
            ? EventBusInterfaceTemplate.TemplateId
            : MessageBusInterfaceTemplate.TemplateId;
    }

    public static string GetBusInterfaceTemplateId(this IIntentTemplate template)
    {
        return GetBusInterfaceTemplateId(template.ExecutionContext);
    }
    
    public static string GetBusVariableName(this ISoftwareFactoryExecutionContext factoryExecutionContext)
    {
        var useLegacy = factoryExecutionContext.Settings.GetEventingSettings().UseLegacyInterfaceName();
        return useLegacy ? "eventBus" : "messageBus";
    }

    public static string GetBusVariableName(this IIntentTemplate template)
    {
        return GetBusVariableName(template.ExecutionContext);
    }
    
    public static bool RequiresCompositeMessageBus(this ISoftwareFactoryExecutionContext factoryExecutionContext)
    {
        var compositeTemplate = factoryExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(
            TemplateDependency.OnTemplate(CompositeMessageBusConfigurationTemplate.TemplateId));
        return compositeTemplate?.CanRunTemplate() == true;
    }

    public static bool RequiresCompositeMessageBus(this IIntentTemplate template)
    {
        return RequiresCompositeMessageBus(template.ExecutionContext);
    }
}