using System;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.Application.NonStringPartitionKeys.DeleteNonStringPartitionKey
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteNonStringPartitionKeyCommandHandler : IRequestHandler<DeleteNonStringPartitionKeyCommand>
    {
        private readonly INonStringPartitionKeyRepository _nonStringPartitionKeyRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteNonStringPartitionKeyCommandHandler(INonStringPartitionKeyRepository nonStringPartitionKeyRepository)
        {
            _nonStringPartitionKeyRepository = nonStringPartitionKeyRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteNonStringPartitionKeyCommand request, CancellationToken cancellationToken)
        {
            var existingNonStringPartitionKey = await _nonStringPartitionKeyRepository.FindByIdAsync((request.Id, request.PartInt), cancellationToken);
            if (existingNonStringPartitionKey is null)
            {
                throw new NotFoundException($"Could not find NonStringPartitionKey '({request.Id}, {request.PartInt})'");
            }

            _nonStringPartitionKeyRepository.Remove(existingNonStringPartitionKey);
        }
    }
}