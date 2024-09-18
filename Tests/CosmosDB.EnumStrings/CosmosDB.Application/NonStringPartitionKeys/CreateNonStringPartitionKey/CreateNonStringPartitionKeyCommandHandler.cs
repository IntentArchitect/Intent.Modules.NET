using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.Application.NonStringPartitionKeys.CreateNonStringPartitionKey
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateNonStringPartitionKeyCommandHandler : IRequestHandler<CreateNonStringPartitionKeyCommand>
    {
        private readonly INonStringPartitionKeyRepository _nonStringPartitionKeyRepository;

        [IntentManaged(Mode.Merge)]
        public CreateNonStringPartitionKeyCommandHandler(INonStringPartitionKeyRepository nonStringPartitionKeyRepository)
        {
            _nonStringPartitionKeyRepository = nonStringPartitionKeyRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateNonStringPartitionKeyCommand request, CancellationToken cancellationToken)
        {
            var newNonStringPartitionKey = new NonStringPartitionKey
            {
                PartInt = request.PartInt,
                Name = request.Name,
            };

            _nonStringPartitionKeyRepository.Add(newNonStringPartitionKey);
        }
    }
}