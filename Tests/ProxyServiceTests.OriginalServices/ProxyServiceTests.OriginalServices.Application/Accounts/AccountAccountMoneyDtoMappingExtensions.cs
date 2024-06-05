using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using ProxyServiceTests.OriginalServices.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace ProxyServiceTests.OriginalServices.Application.Accounts
{
    public static class AccountAccountMoneyDtoMappingExtensions
    {
        public static AccountAccountMoneyDto MapToAccountAccountMoneyDto(this Money projectFrom, IMapper mapper)
            => mapper.Map<AccountAccountMoneyDto>(projectFrom);

        public static List<AccountAccountMoneyDto> MapToAccountAccountMoneyDtoList(this IEnumerable<Money> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToAccountAccountMoneyDto(mapper)).ToList();
    }
}