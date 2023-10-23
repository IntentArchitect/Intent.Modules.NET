using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CosmosDB.EntityInterfaces.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.Clients
{
    public static class ClientDtoMappingExtensions
    {
        public static ClientDto MapToClientDto(this IClient projectFrom, IMapper mapper)
            => mapper.Map<ClientDto>(projectFrom);

        public static List<ClientDto> MapToClientDtoList(this IEnumerable<IClient> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToClientDto(mapper)).ToList();
    }
}