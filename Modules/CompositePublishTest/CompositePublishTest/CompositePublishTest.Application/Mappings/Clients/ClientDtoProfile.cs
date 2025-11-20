using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CompositePublishTest.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace CompositePublishTest.Application.Clients
{
    public class ClientDtoProfile : Profile
    {
        public ClientDtoProfile()
        {
            CreateMap<Client, ClientDto>();
        }
    }

    public static class ClientDtoMappingExtensions
    {
        public static ClientDto MapToClientDto(this Client projectFrom, IMapper mapper) => mapper.Map<ClientDto>(projectFrom);

        public static List<ClientDto> MapToClientDtoList(this IEnumerable<Client> projectFrom, IMapper mapper) => projectFrom.Select(x => x.MapToClientDto(mapper)).ToList();
    }
}