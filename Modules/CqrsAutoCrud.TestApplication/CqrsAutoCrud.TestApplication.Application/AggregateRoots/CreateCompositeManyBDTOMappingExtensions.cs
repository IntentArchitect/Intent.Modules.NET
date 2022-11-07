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
    public static class CreateCompositeManyBDTOMappingExtensions
    {
        public static CreateCompositeManyBDTO MapToCreateCompositeManyBDTO(this ICompositeManyB projectFrom, IMapper mapper)
        {
            return mapper.Map<CreateCompositeManyBDTO>(projectFrom);
        }

        public static List<CreateCompositeManyBDTO> MapToCreateCompositeManyBDTOList(this IEnumerable<ICompositeManyB> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToCreateCompositeManyBDTO(mapper)).ToList();
        }
    }
}