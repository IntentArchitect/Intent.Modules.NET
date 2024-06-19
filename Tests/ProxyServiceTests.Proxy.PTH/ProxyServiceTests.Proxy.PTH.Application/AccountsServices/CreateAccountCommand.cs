using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ProxyServiceTests.Proxy.PTH.Application.AccountsServices
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