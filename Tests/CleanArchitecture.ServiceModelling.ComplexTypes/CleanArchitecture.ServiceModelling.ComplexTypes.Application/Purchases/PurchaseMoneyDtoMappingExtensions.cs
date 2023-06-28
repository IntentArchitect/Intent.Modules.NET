using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.Purchases
{
    public static class PurchaseMoneyDtoMappingExtensions
    {
        public static PurchaseMoneyDto MapToPurchaseMoneyDto(this Money projectFrom, IMapper mapper)
            => mapper.Map<PurchaseMoneyDto>(projectFrom);

        public static List<PurchaseMoneyDto> MapToPurchaseMoneyDtoList(this IEnumerable<Money> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToPurchaseMoneyDto(mapper)).ToList();
    }
}