using AutoMapper;
using Intent.Modules.NET.Tests.Module2.Application.Contracts.Accounts;
using Intent.Modules.NET.Tests.Module2.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module2.Application.Accounts
{
    public class AccountDtoProfile : Profile
    {
        public AccountDtoProfile()
        {
            CreateMap<Account, AccountDto>();
        }
    }

    public static class AccountDtoMappingExtensions
    {
        public static AccountDto MapToAccountDto(this Account projectFrom, IMapper mapper) => mapper.Map<AccountDto>(projectFrom);

        public static List<AccountDto> MapToAccountDtoList(this IEnumerable<Account> projectFrom, IMapper mapper) => projectFrom.Select(x => x.MapToAccountDto(mapper)).ToList();
    }
}