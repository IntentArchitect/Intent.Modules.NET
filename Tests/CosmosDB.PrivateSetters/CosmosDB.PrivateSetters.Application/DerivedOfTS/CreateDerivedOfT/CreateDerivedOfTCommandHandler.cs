using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.DerivedOfTS.CreateDerivedOfT
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateDerivedOfTCommandHandler : IRequestHandler<CreateDerivedOfTCommand, string>
    {
        [IntentManaged(Mode.Merge)]
        public CreateDerivedOfTCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<string> Handle(CreateDerivedOfTCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CreateDerivedOfTCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}