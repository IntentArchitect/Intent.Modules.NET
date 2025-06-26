using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Countries
{
    public static class StateDtoMappingExtensions
    {
        public static StateDto MapToStateDto(this State projectFrom, IMapper mapper)
            => mapper.Map<StateDto>(projectFrom);

        public static List<StateDto> MapToStateDtoList(this IEnumerable<State> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToStateDto(mapper)).ToList();
    }
}