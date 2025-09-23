using System;
using System.Threading;
using System.Threading.Tasks;
using AwsLambdaFunction.Domain.Entities;
using AwsLambdaFunction.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AwsLambdaFunction.Application.EfClients.CreateEfClient
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateEfClientCommandHandler : IRequestHandler<CreateEfClientCommand, Guid>
    {
        private readonly IEfClientRepository _efClientRepository;

        [IntentManaged(Mode.Merge)]
        public CreateEfClientCommandHandler(IEfClientRepository efClientRepository)
        {
            _efClientRepository = efClientRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateEfClientCommand request, CancellationToken cancellationToken)
        {
            var efClient = new EfClient
            {
                Name = request.Name,
                AffiliateId = request.AffiliateId
            };

            _efClientRepository.Add(efClient);
            await _efClientRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return efClient.Id;
        }
    }
}