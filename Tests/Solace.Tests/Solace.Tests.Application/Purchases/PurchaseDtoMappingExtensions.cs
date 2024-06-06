using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Solace.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Solace.Tests.Application.Purchases
{
    public static class PurchaseDtoMappingExtensions
    {
        public static PurchaseDto MapToPurchaseDto(this Purchase projectFrom, IMapper mapper)
            => mapper.Map<PurchaseDto>(projectFrom);

        public static List<PurchaseDto> MapToPurchaseDtoList(this IEnumerable<Purchase> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToPurchaseDto(mapper)).ToList();
    }
}