using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CosmosDB.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CosmosDB.Application.NonStringPartitionKeys
{
    public static class NonStringPartitionKeyDtoMappingExtensions
    {
        public static NonStringPartitionKeyDto MapToNonStringPartitionKeyDto(this NonStringPartitionKey projectFrom, IMapper mapper)
            => mapper.Map<NonStringPartitionKeyDto>(projectFrom);

        public static List<NonStringPartitionKeyDto> MapToNonStringPartitionKeyDtoList(this IEnumerable<NonStringPartitionKey> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToNonStringPartitionKeyDto(mapper)).ToList();
    }
}