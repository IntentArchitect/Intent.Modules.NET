using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.DDD.ChangeAccountHolderName
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ChangeAccountHolderNameHandler : IRequestHandler<ChangeAccountHolderName>
    {
        [IntentManaged(Mode.Ignore)]
        public ChangeAccountHolderNameHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Unit> Handle(ChangeAccountHolderName request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}