using System;
using System.Threading;
using System.Threading.Tasks;
using AwsLambdaFunction.Domain.Common.Exceptions;
using AwsLambdaFunction.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AwsLambdaFunction.Application.DynClients.DeleteDynClient
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteDynClientCommandHandler : IRequestHandler<DeleteDynClientCommand>
    {
        private readonly IDynClientRepository _dynClientRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteDynClientCommandHandler(IDynClientRepository dynClientRepository)
        {
            _dynClientRepository = dynClientRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteDynClientCommand request, CancellationToken cancellationToken)
        {
            var dynClient = await _dynClientRepository.FindByIdAsync(request.Id, cancellationToken);
            if (dynClient is null)
            {
                throw new NotFoundException($"Could not find DynClient '{request.Id}'");
            }


            _dynClientRepository.Remove(dynClient);
        }
    }
}