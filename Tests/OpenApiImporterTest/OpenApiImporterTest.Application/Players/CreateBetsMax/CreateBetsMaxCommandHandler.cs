using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace OpenApiImporterTest.Application.Players.CreateBetsMax
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateBetsMaxCommandHandler : IRequestHandler<CreateBetsMaxCommand, MaxBetResultViewModel>
    {
        [IntentManaged(Mode.Merge)]
        public CreateBetsMaxCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<MaxBetResultViewModel> Handle(CreateBetsMaxCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CreateBetsMaxCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}