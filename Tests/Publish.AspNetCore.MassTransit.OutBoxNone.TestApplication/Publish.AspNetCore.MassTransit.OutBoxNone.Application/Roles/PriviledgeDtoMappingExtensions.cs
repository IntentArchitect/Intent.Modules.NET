using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Publish.AspNetCore.MassTransit.OutBoxNone.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxNone.Application.Roles
{
    public static class PriviledgeDtoMappingExtensions
    {
        public static PriviledgeDto MapToPriviledgeDto(this Priviledge projectFrom, IMapper mapper)
            => mapper.Map<PriviledgeDto>(projectFrom);

        public static List<PriviledgeDto> MapToPriviledgeDtoList(this IEnumerable<Priviledge> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToPriviledgeDto(mapper)).ToList();
    }
}