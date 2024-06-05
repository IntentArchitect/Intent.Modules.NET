using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using ProxyServiceTests.OriginalServices.Application.Common.Mappings;
using ProxyServiceTests.OriginalServices.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ProxyServiceTests.OriginalServices.Application.Accounts
{
    public class AccountDto : IMapFrom<Account>
    {
        public AccountDto()
        {
            Number = null!;
            Amount = null!;
        }

        public Guid Id { get; set; }
        public string Number { get; set; }
        public AccountAccountMoneyDto Amount { get; set; }
        public Guid ClientId { get; set; }

        public static AccountDto Create(Guid id, string number, AccountAccountMoneyDto amount, Guid clientId)
        {
            return new AccountDto
            {
                Id = id,
                Number = number,
                Amount = amount,
                ClientId = clientId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Account, AccountDto>();
        }
    }
}