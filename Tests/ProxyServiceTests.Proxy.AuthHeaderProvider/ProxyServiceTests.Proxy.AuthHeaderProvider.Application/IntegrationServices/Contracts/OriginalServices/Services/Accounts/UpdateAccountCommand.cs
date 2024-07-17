using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace ProxyServiceTests.Proxy.AuthHeaderProvider.Application.IntegrationServices.Contracts.OriginalServices.Services.Accounts
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