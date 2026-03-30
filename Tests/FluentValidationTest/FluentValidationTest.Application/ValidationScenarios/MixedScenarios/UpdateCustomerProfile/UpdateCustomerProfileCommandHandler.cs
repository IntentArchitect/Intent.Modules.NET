using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.MixedScenarios.UpdateCustomerProfile
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateCustomerProfileCommandHandler : IRequestHandler<UpdateCustomerProfileCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateCustomerProfileCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(UpdateCustomerProfileCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (UpdateCustomerProfileCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}