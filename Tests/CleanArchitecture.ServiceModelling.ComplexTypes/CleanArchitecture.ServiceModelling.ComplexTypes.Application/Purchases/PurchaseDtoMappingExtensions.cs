using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.Purchases
{
    public static class PurchaseDtoMappingExtensions
    {
        public static PurchaseDto MapToPurchaseDto(this Purchase projectFrom, IMapper mapper)
            => mapper.Map<PurchaseDto>(projectFrom);

        public static List<PurchaseDto> MapToPurchaseDtoList(this IEnumerable<Purchase> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToPurchaseDto(mapper)).ToList();
    }
}