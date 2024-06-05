using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using ProxyServiceTests.OriginalServices.Application.Common.Mappings;
using ProxyServiceTests.OriginalServices.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ProxyServiceTests.OriginalServices.Application.Accounts
{
    public class AccountAccountMoneyDto : IMapFrom<Money>
    {
        public AccountAccountMoneyDto()
        {
        }

        public decimal Amount { get; set; }
        public Currency Currency { get; set; }

        public static AccountAccountMoneyDto Create(decimal amount, Currency currency)
        {
            return new AccountAccountMoneyDto
            {
                Amount = amount,
                Currency = currency
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Money, AccountAccountMoneyDto>();
        }
    }
}