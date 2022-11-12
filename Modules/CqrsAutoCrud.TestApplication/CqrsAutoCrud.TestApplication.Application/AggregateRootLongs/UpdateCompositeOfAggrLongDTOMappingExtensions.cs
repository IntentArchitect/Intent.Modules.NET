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
    public static class UpdateCompositeOfAggrLongDTOMappingExtensions
    {
        public static UpdateCompositeOfAggrLongDTO MapToUpdateCompositeOfAggrLongDTO(this CompositeOfAggrLong projectFrom, IMapper mapper)
        {
            return mapper.Map<UpdateCompositeOfAggrLongDTO>(projectFrom);
        }

        public static List<UpdateCompositeOfAggrLongDTO> MapToUpdateCompositeOfAggrLongDTOList(this IEnumerable<CompositeOfAggrLong> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToUpdateCompositeOfAggrLongDTO(mapper)).ToList();
        }
    }
}