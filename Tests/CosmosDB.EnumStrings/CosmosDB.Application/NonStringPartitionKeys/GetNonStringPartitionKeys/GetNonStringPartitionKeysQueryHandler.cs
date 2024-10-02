using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CosmosDB.Application.NonStringPartitionKeys.GetNonStringPartitionKeys
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetNonStringPartitionKeysQueryHandler : IRequestHandler<GetNonStringPartitionKeysQuery, List<NonStringPartitionKeyDto>>
    {
        private readonly INonStringPartitionKeyRepository _nonStringPartitionKeyRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetNonStringPartitionKeysQueryHandler(INonStringPartitionKeyRepository nonStringPartitionKeyRepository,
            IMapper mapper)
        {
            _nonStringPartitionKeyRepository = nonStringPartitionKeyRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<NonStringPartitionKeyDto>> Handle(
            GetNonStringPartitionKeysQuery request,
            CancellationToken cancellationToken)
        {
            //IntentIgnore
            var nonStringPartitionKeys = await _nonStringPartitionKeyRepository.FindAllAsync(x => x.PartInt == request.PartInt.ToString(), cancellationToken);
            return nonStringPartitionKeys.MapToNonStringPartitionKeyDtoList(_mapper);
        }
    }
}