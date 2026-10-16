using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Eventing.Wolverine.Templates;

/// <summary>
/// Seam to the Mediator dispatch stack's <c>MessageBusFlushOwner</c> (D18). Referenced by template id only:
/// this module takes no project reference on <c>Intent.Application.Mediator</c>.
/// </summary>
public static class MediatorFlushOwnerExtensions
{
    public const string MessageBusFlushOwnerTemplateId = "Intent.Application.Mediator.MessageBusFlushOwner";

    /// <summary>
    /// True when this application dispatches through Mediator and a message bus is registered for this broker,
    /// i.e. when an Outbox = None consume path has an owner to claim and something for the owner to guard.
    /// <para>
    /// <c>FindTemplateInstance</c> alone is not enough: the owner template belongs to an always-installed module
    /// and resolves its type name even when its own <c>CanRunTemplate()</c> skips it (no message bus role resolved),
    /// so the instance's <c>CanRunTemplate()</c> is checked as well.
    /// </para>
    /// </summary>
    public static bool TryGetMediatorFlushOwner(this ICSharpFileBuilderTemplate template, out string ownerTypeName)
    {
        ownerTypeName = string.Empty;

        var owner = template.ExecutionContext.FindTemplateInstance<IIntentTemplate>(MessageBusFlushOwnerTemplateId);
        if (owner == null || !owner.CanRunTemplate())
        {
            return false;
        }

        // A subscribe-only application registers no bus (R3.9), and the Mediator pipeline's flush item is stripped
        // for it, so there is no buffer to guard.
        if (!template.WolverineMessageBusIsRegistered())
        {
            return false;
        }

        return template.TryGetTypeName(MessageBusFlushOwnerTemplateId, out ownerTypeName);
    }
}
