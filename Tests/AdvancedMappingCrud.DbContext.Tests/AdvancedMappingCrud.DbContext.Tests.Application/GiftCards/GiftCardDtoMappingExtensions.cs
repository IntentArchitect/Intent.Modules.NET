using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.DbContext.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.GiftCards
{
    public static class GiftCardDtoMappingExtensions
    {
        public static GiftCardDto MapToGiftCardDto(this GiftCard projectFrom, IMapper mapper)
            => mapper.Map<GiftCardDto>(projectFrom);

        public static List<GiftCardDto> MapToGiftCardDtoList(this IEnumerable<GiftCard> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToGiftCardDto(mapper)).ToList();
    }
}