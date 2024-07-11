using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.SimpleOdata
{
    public static class SimpleOdataDtoMappingExtensions
    {
        public static SimpleOdataDto MapToSimpleOdataDto(this Domain.Entities.SimpleOdata projectFrom, IMapper mapper)
            => mapper.Map<SimpleOdataDto>(projectFrom);

        public static List<SimpleOdataDto> MapToSimpleOdataDtoList(this IEnumerable<Domain.Entities.SimpleOdata> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToSimpleOdataDto(mapper)).ToList();
    }
}