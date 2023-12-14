using System;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.EntityInterfaces.Domain.Common.Exceptions;
using CosmosDB.EntityInterfaces.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.EntityInterfaces.Application.NonStringPartitionKeys.UpdateNonStringPartitionKey
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
            var nonStringPartitionKey = await _nonStringPartitionKeyRepository.FindByIdAsync((request.Id, request.PartInt), cancellationToken);
            if (nonStringPartitionKey is null)
            {
                throw new NotFoundException($"Could not find NonStringPartitionKey '({request.Id}, {request.PartInt})'");
            }

            nonStringPartitionKey.PartInt = request.PartInt;
            nonStringPartitionKey.Name = request.Name;

            _nonStringPartitionKeyRepository.Update(nonStringPartitionKey);
        }
    }
}