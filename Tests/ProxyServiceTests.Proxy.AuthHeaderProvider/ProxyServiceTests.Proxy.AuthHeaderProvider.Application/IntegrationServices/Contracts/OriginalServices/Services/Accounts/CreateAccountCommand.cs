using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace ProxyServiceTests.Proxy.AuthHeaderProvider.Application.IntegrationServices.Contracts.OriginalServices.Services.Accounts
{
    public class CreateAccountCommand
    {
        public CreateAccountCommand()
        {
            Number = null!;
            Amount = null!;
        }

        public string Number { get; set; }
        public CreateAccountMoneyDto Amount { get; set; }
        public Guid ClientId { get; set; }

        public static CreateAccountCommand Create(string number, CreateAccountMoneyDto amount, Guid clientId)
        {
            return new CreateAccountCommand
            {
                Number = number,
                Amount = amount,
                ClientId = clientId
            };
        }
    }
}