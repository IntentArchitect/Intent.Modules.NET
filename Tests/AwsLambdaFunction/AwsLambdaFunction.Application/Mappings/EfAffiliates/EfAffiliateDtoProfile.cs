using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AwsLambdaFunction.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace AwsLambdaFunction.Application.EfAffiliates
{
    public class EfAffiliateDtoProfile : Profile
    {
        public EfAffiliateDtoProfile()
        {
            CreateMap<EfAffiliate, EfAffiliateDto>();
        }
    }

    public static class EfAffiliateDtoMappingExtensions
    {
        public static EfAffiliateDto MapToEfAffiliateDto(this EfAffiliate projectFrom, IMapper mapper) => mapper.Map<EfAffiliateDto>(projectFrom);

        public static List<EfAffiliateDto> MapToEfAffiliateDtoList(
            this IEnumerable<EfAffiliate> projectFrom,
            IMapper mapper) => projectFrom.Select(x => x.MapToEfAffiliateDto(mapper)).ToList();
    }
}