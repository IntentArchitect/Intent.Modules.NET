using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.OriginalServices.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace ProxyServiceTests.OriginalServices.Application.Accounts.CreateAccount
{
    public class CreateAccountCommand : IRequest<Guid>, ICommand
    {
        public CreateAccountCommand(CreateAccountMoneyDto amount, Guid clientId, string number = "00")
        {
            Amount = amount;
            ClientId = clientId;
            Number = number;
        }

        public string Number { get; set; }
        public CreateAccountMoneyDto Amount { get; set; }
        public Guid ClientId { get; set; }
    }
}