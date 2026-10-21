using System.Reflection;
using Intent.RoslynWeaver.Attributes;
using Mediator;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.AuthorizationBehaviour", Version = "1.0")]

namespace TestMediator.WolverineSubscribeOnly.Infrastructure.Dispatch.Behaviours
{
    public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : global::Mediator.IMessage
    {
        public ValueTask<TResponse> Handle(
            TRequest request,
            MessageHandlerDelegate<TRequest, TResponse> next,
            CancellationToken cancellationToken)
        {
            return next(request, cancellationToken);
        }
    }
}