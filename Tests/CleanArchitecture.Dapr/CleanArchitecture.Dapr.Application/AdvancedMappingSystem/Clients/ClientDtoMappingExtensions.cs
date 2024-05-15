using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Dapr.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.AdvancedMappingSystem.Clients
{
    public static class ClientDtoMappingExtensions
    {
        public static ClientDto MapToClientDto(this Client projectFrom, IMapper mapper)
            => mapper.Map<ClientDto>(projectFrom);

        public static List<ClientDto> MapToClientDtoList(this IEnumerable<Client> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToClientDto(mapper)).ToList();
    }
}