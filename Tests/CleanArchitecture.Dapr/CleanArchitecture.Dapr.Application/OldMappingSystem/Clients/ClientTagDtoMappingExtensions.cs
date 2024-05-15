using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Dapr.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Clients
{
    public static class ClientTagDtoMappingExtensions
    {
        public static ClientTagDto MapToClientTagDto(this Tag projectFrom, IMapper mapper)
            => mapper.Map<ClientTagDto>(projectFrom);

        public static List<ClientTagDto> MapToClientTagDtoList(this IEnumerable<Tag> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToClientTagDto(mapper)).ToList();
    }
}