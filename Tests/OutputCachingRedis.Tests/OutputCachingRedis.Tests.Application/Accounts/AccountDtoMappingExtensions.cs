using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using OutputCachingRedis.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace OutputCachingRedis.Tests.Application.Accounts
{
    public static class AccountDtoMappingExtensions
    {
        public static AccountDto MapToAccountDto(this Account projectFrom, IMapper mapper)
            => mapper.Map<AccountDto>(projectFrom);

        public static List<AccountDto> MapToAccountDtoList(this IEnumerable<Account> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToAccountDto(mapper)).ToList();
    }
}