using System;
using System.Threading;
using System.Threading.Tasks;
using AwsLambdaFunction.Domain.Entities;
using AwsLambdaFunction.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AwsLambdaFunction.Application.DynClients.CreateDynClient
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateDynClientCommandHandler : IRequestHandler<CreateDynClientCommand, string>
    {
        private readonly IDynClientRepository _dynClientRepository;

        [IntentManaged(Mode.Merge)]
        public CreateDynClientCommandHandler(IDynClientRepository dynClientRepository)
        {
            _dynClientRepository = dynClientRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateDynClientCommand request, CancellationToken cancellationToken)
        {
            var dynClient = new DynClient
            {
                Name = request.Name,
                AffiliateId = request.AffiliateId
            };

            _dynClientRepository.Add(dynClient);
            await _dynClientRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return dynClient.Id;
        }
    }
}