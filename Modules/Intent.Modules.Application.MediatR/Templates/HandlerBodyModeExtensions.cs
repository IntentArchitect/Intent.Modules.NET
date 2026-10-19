using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Application.MediatR.Templates;

internal static class HandlerBodyModeExtensions
{
    /// <summary>
    /// Modules which implement MediatR handler bodies from the model. Intent.Application.MediatR.CRUD finds
    /// handlers by this module's TemplateId, Intent.Application.CQRS.CRUD by the handler TemplateRoles.
    /// </summary>
    private static readonly string[] CrudImplementationModuleIds =
    [
        "Intent.Application.MediatR.CRUD",
        "Intent.Application.CQRS.CRUD"
    ];

    public static bool IsCrudImplementationInstalled(this ISoftwareFactoryExecutionContext executionContext)
    {
        return executionContext.InstalledModules.Any(m => CrudImplementationModuleIds.Contains(m.ModuleId));
    }

    /// <summary>
    /// With a CRUD module installed this is the long-standing Merge default, which a CRUD module overrides to
    /// Fully on the handlers it implements. Without one, nothing will ever generate the body, so it is handed
    /// over entirely (Ignore) from the first run.
    /// </summary>
    public static CSharpIntentManagedAttribute GetHandleMethodIntentManagedAttribute(this ISoftwareFactoryExecutionContext executionContext)
    {
        return executionContext.IsCrudImplementationInstalled()
            ? CSharpIntentManagedAttribute.Fully().WithBodyMerge()
            : CSharpIntentManagedAttribute.Fully().WithBodyIgnored();
    }
}
