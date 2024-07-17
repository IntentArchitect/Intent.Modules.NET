using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.Proxy.AuthHeaderProvider.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace ProxyServiceTests.Proxy.AuthHeaderProvider.Application.AccountsServices.UpdateAccount
{
    public class UpdateAccountCommand : IRequest, ICommand
    {
        public UpdateAccountCommand(Guid id,
            string number,
            UpdateAccountMoneyDto amount,
            Guid clientId,
            Guid updateAccountCommandId)
        {
            Id = id;
            Number = number;
            Amount = amount;
            ClientId = clientId;
            UpdateAccountCommandId = updateAccountCommandId;
        }

        public Guid Id { get; set; }
        public string Number { get; set; }
        public UpdateAccountMoneyDto Amount { get; set; }
        public Guid ClientId { get; set; }
        public Guid UpdateAccountCommandId { get; set; }
    }
}