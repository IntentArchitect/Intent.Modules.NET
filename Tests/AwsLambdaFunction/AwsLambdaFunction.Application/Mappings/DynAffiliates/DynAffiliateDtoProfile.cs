using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AwsLambdaFunction.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace AwsLambdaFunction.Application.DynAffiliates
{
    public class DynAffiliateDtoProfile : Profile
    {
        public DynAffiliateDtoProfile()
        {
            CreateMap<DynAffiliate, DynAffiliateDto>();
        }
    }

    public static class DynAffiliateDtoMappingExtensions
    {
        public static DynAffiliateDto MapToDynAffiliateDto(this DynAffiliate projectFrom, IMapper mapper) => mapper.Map<DynAffiliateDto>(projectFrom);

        public static List<DynAffiliateDto> MapToDynAffiliateDtoList(
            this IEnumerable<DynAffiliate> projectFrom,
            IMapper mapper) => projectFrom.Select(x => x.MapToDynAffiliateDto(mapper)).ToList();
    }
}