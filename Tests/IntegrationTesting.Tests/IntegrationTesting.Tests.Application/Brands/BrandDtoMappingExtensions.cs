using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Brands
{
    public static class BrandDtoMappingExtensions
    {
        public static BrandDto MapToBrandDto(this Brand projectFrom, IMapper mapper)
            => mapper.Map<BrandDto>(projectFrom);

        public static List<BrandDto> MapToBrandDtoList(this IEnumerable<Brand> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToBrandDto(mapper)).ToList();
    }
}