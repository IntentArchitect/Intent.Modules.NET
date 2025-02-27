using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.DomainInvoke;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Farmers
{
    public static class FarmerDtoMappingExtensions
    {
        public static FarmerDto MapToFarmerDto(this Farmer projectFrom, IMapper mapper)
            => mapper.Map<FarmerDto>(projectFrom);

        public static List<FarmerDto> MapToFarmerDtoList(this IEnumerable<Farmer> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToFarmerDto(mapper)).ToList();
    }
}