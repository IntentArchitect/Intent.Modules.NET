using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CosmosDB.Application.NonStringPartitionKeys.GetNonStringPartitionKeyById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetNonStringPartitionKeyByIdQueryHandler : IRequestHandler<GetNonStringPartitionKeyByIdQuery, NonStringPartitionKeyDto>
    {
        private readonly INonStringPartitionKeyRepository _nonStringPartitionKeyRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetNonStringPartitionKeyByIdQueryHandler(INonStringPartitionKeyRepository nonStringPartitionKeyRepository,
            IMapper mapper)
        {
            _nonStringPartitionKeyRepository = nonStringPartitionKeyRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<NonStringPartitionKeyDto> Handle(
            GetNonStringPartitionKeyByIdQuery request,
            CancellationToken cancellationToken)
        {
            var nonStringPartitionKey = await _nonStringPartitionKeyRepository.FindByIdAsync((request.Id, request.PartInt), cancellationToken);
            if (nonStringPartitionKey is null)
            {
                throw new NotFoundException($"Could not find NonStringPartitionKey '({request.Id}, {request.PartInt})'");
            }

            return nonStringPartitionKey.MapToNonStringPartitionKeyDto(_mapper);
        }
    }
}