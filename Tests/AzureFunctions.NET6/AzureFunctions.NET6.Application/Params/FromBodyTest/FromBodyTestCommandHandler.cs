using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AzureFunctions.NET6.Application.Params.FromBodyTest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class FromBodyTestCommandHandler : IRequestHandler<FromBodyTestCommand>
    {
        [IntentManaged(Mode.Merge)]
        public FromBodyTestCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(FromBodyTestCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}