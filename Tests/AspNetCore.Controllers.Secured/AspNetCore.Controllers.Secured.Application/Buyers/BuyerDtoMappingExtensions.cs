using System.Collections.Generic;
using System.Linq;
using AspNetCore.Controllers.Secured.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AspNetCore.Controllers.Secured.Application.Buyers
{
    public static class BuyerDtoMappingExtensions
    {
        public static BuyerDto MapToBuyerDto(this Buyer projectFrom, IMapper mapper)
            => mapper.Map<BuyerDto>(projectFrom);

        public static List<BuyerDto> MapToBuyerDtoList(this IEnumerable<Buyer> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToBuyerDto(mapper)).ToList();
    }
}