using System;
using System.Threading;
using System.Threading.Tasks;
using AwsLambdaFunction.Domain.Common.Exceptions;
using AwsLambdaFunction.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AwsLambdaFunction.Application.DynClients.UpdateDynClient
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateDynClientCommandHandler : IRequestHandler<UpdateDynClientCommand>
    {
        private readonly IDynClientRepository _dynClientRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateDynClientCommandHandler(IDynClientRepository dynClientRepository)
        {
            _dynClientRepository = dynClientRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateDynClientCommand request, CancellationToken cancellationToken)
        {
            var dynClient = await _dynClientRepository.FindByIdAsync(request.Id, cancellationToken);
            if (dynClient is null)
            {
                throw new NotFoundException($"Could not find DynClient '{request.Id}'");
            }

            dynClient.Name = request.Name;
            dynClient.AffiliateId = request.AffiliateId;

            _dynClientRepository.Update(dynClient);
        }
    }
}