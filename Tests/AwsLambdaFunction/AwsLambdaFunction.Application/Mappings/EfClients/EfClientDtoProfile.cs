using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AwsLambdaFunction.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace AwsLambdaFunction.Application.EfClients
{
    public class EfClientDtoProfile : Profile
    {
        public EfClientDtoProfile()
        {
            CreateMap<EfClient, EfClientDto>();
        }
    }

    public static class EfClientDtoMappingExtensions
    {
        public static EfClientDto MapToEfClientDto(this EfClient projectFrom, IMapper mapper) => mapper.Map<EfClientDto>(projectFrom);

        public static List<EfClientDto> MapToEfClientDtoList(this IEnumerable<EfClient> projectFrom, IMapper mapper) => projectFrom.Select(x => x.MapToEfClientDto(mapper)).ToList();
    }
}