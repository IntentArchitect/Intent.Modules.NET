using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CosmosDB.EntityInterfaces.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.NonStringPartitionKeys
{
    public static class NonStringPartitionKeyDtoMappingExtensions
    {
        public static NonStringPartitionKeyDto MapToNonStringPartitionKeyDto(this INonStringPartitionKey projectFrom, IMapper mapper)
            => mapper.Map<NonStringPartitionKeyDto>(projectFrom);

        public static List<NonStringPartitionKeyDto> MapToNonStringPartitionKeyDtoList(this IEnumerable<INonStringPartitionKey> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToNonStringPartitionKeyDto(mapper)).ToList();
    }
}