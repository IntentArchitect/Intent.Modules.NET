using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AwsLambdaFunction.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace AwsLambdaFunction.Application.DynClients
{
    public class DynClientDtoProfile : Profile
    {
        public DynClientDtoProfile()
        {
            CreateMap<DynClient, DynClientDto>();
        }
    }

    public static class DynClientDtoMappingExtensions
    {
        public static DynClientDto MapToDynClientDto(this DynClient projectFrom, IMapper mapper) => mapper.Map<DynClientDto>(projectFrom);

        public static List<DynClientDto> MapToDynClientDtoList(this IEnumerable<DynClient> projectFrom, IMapper mapper) => projectFrom.Select(x => x.MapToDynClientDto(mapper)).ToList();
    }
}