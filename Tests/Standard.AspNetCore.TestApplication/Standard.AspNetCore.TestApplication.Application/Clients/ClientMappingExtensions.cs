using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Clients
{
    public static class ClientMappingExtensions
    {
        public static Client MapToClient(this Domain.Entities.Client projectFrom, IMapper mapper)
            => mapper.Map<Client>(projectFrom);

        public static List<Client> MapToClientList(this IEnumerable<Domain.Entities.Client> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToClient(mapper)).ToList();
    }
}