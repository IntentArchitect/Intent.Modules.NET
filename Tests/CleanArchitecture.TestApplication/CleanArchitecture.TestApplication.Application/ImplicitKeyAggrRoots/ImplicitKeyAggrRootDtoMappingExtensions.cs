using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using CleanArchitecture.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots
{
    public static class ImplicitKeyAggrRootDtoMappingExtensions
    {
        public static ImplicitKeyAggrRootDto MapToImplicitKeyAggrRootDto(this ImplicitKeyAggrRoot projectFrom, IMapper mapper)
        {
            return mapper.Map<ImplicitKeyAggrRootDto>(projectFrom);
        }

        public static List<ImplicitKeyAggrRootDto> MapToImplicitKeyAggrRootDtoList(this IEnumerable<ImplicitKeyAggrRoot> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToImplicitKeyAggrRootDto(mapper)).ToList();
        }
    }
}