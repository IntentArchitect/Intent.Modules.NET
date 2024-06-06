using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Solace.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Solace.Tests.Application.Purchases.UpdatePurchase
{
    public class UpdatePurchaseCommand : IRequest, ICommand
    {
        public UpdatePurchaseCommand(Guid accountId, decimal amount, Guid id)
        {
            AccountId = accountId;
            Amount = amount;
            Id = id;
        }

        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public Guid Id { get; set; }
    }
}