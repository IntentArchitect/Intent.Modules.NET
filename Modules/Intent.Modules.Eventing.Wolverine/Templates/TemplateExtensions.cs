using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Wolverine.Templates.WolverineCompositeConfiguration;
using Intent.Modules.Eventing.Wolverine.Templates.WolverineMessageBus;
using Intent.Modules.Eventing.Wolverine.Templates.WolverineTenantMiddleware;
using Intent.Modules.Eventing.Wolverine.Templates.WolverineTenantStrategy;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.Wolverine.Templates;

public static class TemplateExtensions
{
    public static string GetWolverineCompositeConfigurationName(this IIntentTemplate template)
    {
        return template.GetTypeName(WolverineCompositeConfigurationTemplate.TemplateId);
    }

    public static string GetWolverineMessageBusName(this IIntentTemplate template)
    {
        return template.GetTypeName(WolverineMessageBusTemplate.TemplateId);
    }

    public static string GetWolverineTenantMiddlewareName(this IIntentTemplate template)
    {
        return template.GetTypeName(WolverineTenantMiddlewareTemplate.TemplateId);
    }

    public static string GetWolverineTenantStrategyName(this IIntentTemplate template)
    {
        return template.GetTypeName(WolverineTenantStrategyTemplate.TemplateId);
    }
}
