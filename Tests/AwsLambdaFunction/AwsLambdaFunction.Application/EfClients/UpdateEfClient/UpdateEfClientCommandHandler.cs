using System;
using System.Threading;
using System.Threading.Tasks;
using AwsLambdaFunction.Domain.Common.Exceptions;
using AwsLambdaFunction.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AwsLambdaFunction.Application.EfClients.UpdateEfClient
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateEfClientCommandHandler : IRequestHandler<UpdateEfClientCommand>
    {
        private readonly IEfClientRepository _efClientRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateEfClientCommandHandler(IEfClientRepository efClientRepository)
        {
            _efClientRepository = efClientRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateEfClientCommand request, CancellationToken cancellationToken)
        {
            var efClient = await _efClientRepository.FindByIdAsync(request.Id, cancellationToken);
            if (efClient is null)
            {
                throw new NotFoundException($"Could not find EfClient '{request.Id}'");
            }

            efClient.Name = request.Name;
            efClient.AffiliateId = request.AffiliateId;
        }
    }
}