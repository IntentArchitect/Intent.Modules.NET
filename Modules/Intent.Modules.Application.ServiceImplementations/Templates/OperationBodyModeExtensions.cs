using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Application.ServiceImplementations.Templates;

public static class OperationBodyModeExtensions
{
    /// <summary>
    /// Modules which implement service operation bodies from the model. Intent.Application.ServiceImplementations.Conventions.CRUD
    /// implements the operation on the service implementation, or the Service Call Handler's Handle method when
    /// Intent.Application.ServiceCallHandlers is installed.
    /// </summary>
    private static readonly string[] CrudImplementationModuleIds =
    [
        "Intent.Application.ServiceImplementations.Conventions.CRUD"
    ];

    private const string ServiceCallHandlersModuleId = "Intent.Application.ServiceCallHandlers";

    public static bool IsCrudImplementationInstalled(this ISoftwareFactoryExecutionContext executionContext)
    {
        return executionContext.InstalledModules.Any(m => CrudImplementationModuleIds.Contains(m.ModuleId));
    }

    /// <summary>
    /// With a CRUD module installed this is the long-standing Merge default, which a CRUD module overrides to
    /// Fully on the operations it implements. Without one, nothing will ever generate the body, so it is handed
    /// over entirely (Ignore) from the first run.
    /// </summary>
    public static CSharpIntentManagedAttribute GetOperationMethodIntentManagedAttribute(this ISoftwareFactoryExecutionContext executionContext)
    {
        return executionContext.IsCrudImplementationInstalled()
            ? CSharpIntentManagedAttribute.Fully().WithBodyMerge()
            : CSharpIntentManagedAttribute.Fully().WithBodyIgnored();
    }

    /// <summary>
    /// With Service Call Handlers the service class only dispatches to each handler's Handle method. The dispatcher
    /// is generated <c>Body = Mode.Fully</c> and must stay that way, which the skill's "switch to Mode.Ignore" rule
    /// would otherwise contradict. Returns an empty string when Intent.Application.ServiceCallHandlers is not installed.
    /// </summary>
    public static string GetServiceCallHandlerDispatcherSkillRule(this ISoftwareFactoryExecutionContext executionContext)
    {
        if (!executionContext.InstalledModules.Any(m => m.ModuleId == ServiceCallHandlersModuleId))
        {
            return string.Empty;
        }

        return """

               - A service operation that only dispatches to a Service Call Handler (`var sch = ...GetRequiredService(...)`) must stay `Body = Mode.Fully`. Leave it unchanged and implement the handler's `Handle` method instead; the rule above applies to `Handle`.
               """;
    }
}
