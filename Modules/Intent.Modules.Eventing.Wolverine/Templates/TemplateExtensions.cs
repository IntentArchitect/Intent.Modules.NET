using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Wolverine.Templates.WolverineEventingConfiguration;
using Intent.Modules.Eventing.Wolverine.Templates.WolverineMessageBus;
using Intent.Modules.Eventing.Wolverine.Templates.WolverineTenantHeaderStrategy;
using Intent.Modules.Eventing.Wolverine.Templates.WolverineTenantMiddleware;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.Wolverine.Templates;

public static class TemplateExtensions
{
    public static string GetWolverineEventingConfigurationName(this IIntentTemplate template)
    {
        return template.GetTypeName(WolverineEventingConfigurationTemplate.TemplateId);
    }

    public static string GetWolverineMessageBusName(this IIntentTemplate template)
    {
        return template.GetTypeName(WolverineMessageBusTemplate.TemplateId);
    }

    public static string GetWolverineTenantHeaderStrategyName(this IIntentTemplate template)
    {
        return template.GetTypeName(WolverineTenantHeaderStrategyTemplate.TemplateId);
    }

    public static string GetWolverineTenantMiddlewareName(this IIntentTemplate template)
    {
        return template.GetTypeName(WolverineTenantMiddlewareTemplate.TemplateId);
    }
}
