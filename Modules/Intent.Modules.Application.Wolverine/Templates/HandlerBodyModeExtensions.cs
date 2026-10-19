using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Application.Wolverine.Templates;

internal static class HandlerBodyModeExtensions
{
    /// <summary>
    /// The only module which implements Wolverine handler bodies from the model. It finds handlers by the
    /// handler TemplateRoles; Intent.Application.MediatR.CRUD matches the MediatR TemplateId and never touches these.
    /// </summary>
    private const string CrudImplementationModuleId = "Intent.Application.CQRS.CRUD";

    public static bool IsCrudImplementationInstalled(this ISoftwareFactoryExecutionContext executionContext)
    {
        return executionContext.InstalledModules.Any(m => m.ModuleId == CrudImplementationModuleId);
    }

    /// <summary>
    /// With the CRUD module installed this is the long-standing Merge default, which the CRUD module overrides to
    /// Fully on the handlers it implements. Without it, nothing will ever generate the body, so it is handed
    /// over entirely (Ignore) from the first run.
    /// </summary>
    public static CSharpIntentManagedAttribute GetHandleMethodIntentManagedAttribute(this ISoftwareFactoryExecutionContext executionContext)
    {
        return executionContext.IsCrudImplementationInstalled()
            ? CSharpIntentManagedAttribute.Fully().WithBodyMerge()
            : CSharpIntentManagedAttribute.Fully().WithBodyIgnored();
    }
}
