using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.OriginalServices.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace ProxyServiceTests.OriginalServices.Application.Accounts.UpdateAccount
{
    public class UpdateAccountCommand : IRequest, ICommand
    {
        public UpdateAccountCommand(string number, UpdateAccountMoneyDto amount, Guid clientId, Guid id)
        {
            Number = number;
            Amount = amount;
            ClientId = clientId;
            Id = id;
        }

        public string Number { get; set; }
        public UpdateAccountMoneyDto Amount { get; set; }
        public Guid ClientId { get; set; }
        public Guid Id { get; set; }
    }
}