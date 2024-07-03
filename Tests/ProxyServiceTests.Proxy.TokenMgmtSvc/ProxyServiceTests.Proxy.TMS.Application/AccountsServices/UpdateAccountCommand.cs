using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ProxyServiceTests.Proxy.TMS.Application.AccountsServices
{
    public class UpdateAccountCommand
    {
        public UpdateAccountCommand()
        {
            Number = null!;
            Amount = null!;
        }

        public string Number { get; set; }
        public UpdateAccountMoneyDto Amount { get; set; }
        public Guid ClientId { get; set; }
        public Guid Id { get; set; }

        public static UpdateAccountCommand Create(string number, UpdateAccountMoneyDto amount, Guid clientId, Guid id)
        {
            return new UpdateAccountCommand
            {
                Number = number,
                Amount = amount,
                ClientId = clientId,
                Id = id
            };
        }
    }
}