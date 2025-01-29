using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace ProxyServiceTests.Proxy.PTH.Application.IntegrationServices.Contracts.OriginalServices.Services.Accounts
{
    public class CreateAccountCommand
    {
        public CreateAccountCommand()
        {
            Amount = null!;
        }

        public string Number { get; set; }
        public CreateAccountMoneyDto Amount { get; set; }
        public Guid ClientId { get; set; }

        public static CreateAccountCommand Create(CreateAccountMoneyDto amount, Guid clientId, string number = "00")
        {
            return new CreateAccountCommand
            {
                Amount = amount,
                ClientId = clientId,
                Number = number
            };
        }
    }
}