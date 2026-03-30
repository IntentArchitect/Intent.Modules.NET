using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.RecursiveDtos.ValidateRecursiveNode
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ValidateRecursiveNodeCommandHandler : IRequestHandler<ValidateRecursiveNodeCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ValidateRecursiveNodeCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(ValidateRecursiveNodeCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (ValidateRecursiveNodeCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}