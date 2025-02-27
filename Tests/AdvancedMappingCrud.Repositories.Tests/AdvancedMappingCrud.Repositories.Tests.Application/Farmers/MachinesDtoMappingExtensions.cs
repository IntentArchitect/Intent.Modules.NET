using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.DomainInvoke;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Farmers
{
    public static class MachinesDtoMappingExtensions
    {
        public static MachinesDto MapToMachinesDto(this Machines projectFrom, IMapper mapper)
            => mapper.Map<MachinesDto>(projectFrom);

        public static List<MachinesDto> MapToMachinesDtoList(this IEnumerable<Machines> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToMachinesDto(mapper)).ToList();
    }
}