using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CosmosDB.EntityInterfaces.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.ClassContainers
{
    public static class ClassContainerDtoMappingExtensions
    {
        public static ClassContainerDto MapToClassContainerDto(this IClassContainer projectFrom, IMapper mapper)
            => mapper.Map<ClassContainerDto>(projectFrom);

        public static List<ClassContainerDto> MapToClassContainerDtoList(this IEnumerable<IClassContainer> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToClassContainerDto(mapper)).ToList();
    }
}