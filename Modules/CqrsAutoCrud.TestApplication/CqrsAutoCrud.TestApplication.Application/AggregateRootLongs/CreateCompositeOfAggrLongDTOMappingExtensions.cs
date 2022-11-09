using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootLongs
{
    public static class CreateCompositeOfAggrLongDTOMappingExtensions
    {
        public static CreateCompositeOfAggrLongDTO MapToCreateCompositeOfAggrLongDTO(this CompositeOfAggrLong projectFrom, IMapper mapper)
        {
            return mapper.Map<CreateCompositeOfAggrLongDTO>(projectFrom);
        }

        public static List<CreateCompositeOfAggrLongDTO> MapToCreateCompositeOfAggrLongDTOList(this IEnumerable<CompositeOfAggrLong> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToCreateCompositeOfAggrLongDTO(mapper)).ToList();
        }
    }
}