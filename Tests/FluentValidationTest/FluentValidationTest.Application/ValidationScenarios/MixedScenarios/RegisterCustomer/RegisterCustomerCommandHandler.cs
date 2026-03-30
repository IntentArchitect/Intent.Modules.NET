using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.MixedScenarios.RegisterCustomer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand>
    {
        [IntentManaged(Mode.Merge)]
        public RegisterCustomerCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (RegisterCustomerCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}