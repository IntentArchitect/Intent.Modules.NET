using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots
{
    public static class CreateCompositeManyAADTOMappingExtensions
    {
        public static CreateCompositeManyAADTO MapToCreateCompositeManyAADTO(this ICompositeManyAA projectFrom, IMapper mapper)
        {
            return mapper.Map<CreateCompositeManyAADTO>(projectFrom);
        }

        public static List<CreateCompositeManyAADTO> MapToCreateCompositeManyAADTOList(this IEnumerable<ICompositeManyAA> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToCreateCompositeManyAADTO(mapper)).ToList();
        }
    }
}