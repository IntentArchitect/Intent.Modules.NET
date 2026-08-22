using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.FluentValidation.BypassValidationInterface", Version = "1.0")]

namespace Wolverine.Publish.RabbitMQ.Application.Common.Interfaces
{
    /// <summary>
    /// Defines a marker interface that, when implemented by a message, instructs the
    /// <c>ValidationMiddleware</c> to skip the execution of all registered validators.
    /// </summary>
    /// <remarks>
    /// Use this interface for specialized messages where standard validation
    /// is redundant or must be deferred to a later stage of processing.
    /// </remarks>
    public interface IBypassValidation
    {
    }
}