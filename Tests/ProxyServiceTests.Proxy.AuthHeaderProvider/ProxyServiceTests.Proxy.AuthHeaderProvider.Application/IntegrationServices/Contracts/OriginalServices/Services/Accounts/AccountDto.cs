using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace ProxyServiceTests.Proxy.AuthHeaderProvider.Application.IntegrationServices.Contracts.OriginalServices.Services.Accounts
{
    public class AccountDto
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
    }
}