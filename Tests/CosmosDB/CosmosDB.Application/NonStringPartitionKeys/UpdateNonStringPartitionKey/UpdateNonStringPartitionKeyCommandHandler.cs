using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.Application.NonStringPartitionKeys.UpdateNonStringPartitionKey
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateNonStringPartitionKeyCommandHandler : IRequestHandler<UpdateNonStringPartitionKeyCommand>
    {
        private readonly INonStringPartitionKeyRepository _nonStringPartitionKeyRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateNonStringPartitionKeyCommandHandler(INonStringPartitionKeyRepository nonStringPartitionKeyRepository)
        {
            _nonStringPartitionKeyRepository = nonStringPartitionKeyRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateNonStringPartitionKeyCommand request, CancellationToken cancellationToken)
        {
            var existingNonStringPartitionKey = await _nonStringPartitionKeyRepository.FindByIdAsync((request.Id, request.PartInt), cancellationToken);
            if (existingNonStringPartitionKey is null)
            {
                throw new NotFoundException($"Could not find NonStringPartitionKey '({request.Id}, {request.PartInt})'");
            }

            existingNonStringPartitionKey.PartInt = request.PartInt;
            existingNonStringPartitionKey.Name = request.Name;

            _nonStringPartitionKeyRepository.Update(existingNonStringPartitionKey);
        }
    }
}