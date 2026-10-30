using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.FluentValidation.BypassValidationInterface", Version = "1.0")]

namespace TestMediator.Nsb.Application.Common.Interfaces
{
    /// <summary>
    /// Defines a marker interface that, when implemented by a command or query, instructs the
    /// <c>ValidationBehaviour&lt;TRequest, TResponse&gt;</c> to skip the execution of all registered validators.
    /// </summary>
    /// <remarks>
    /// Use this interface for specialized messages where standard pipeline validation
    /// is redundant or must be deferred to a later stage of processing.
    /// </remarks>
    public interface IBypassValidation
    {
    }
}