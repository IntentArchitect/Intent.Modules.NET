using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace ProxyServiceTests.Proxy.AuthHeaderProvider.Application.AccountsServices.CreateAccount
{
    public class CreateAccountCommand : IRequest<Guid>, ICommand
    {
        public CreateAccountCommand(string number, CreateAccountMoneyDto amount, Guid clientId)
        {
            Number = number;
            Amount = amount;
            ClientId = clientId;
        }

        public string Number { get; set; }
        public CreateAccountMoneyDto Amount { get; set; }
        public Guid ClientId { get; set; }
    }
}