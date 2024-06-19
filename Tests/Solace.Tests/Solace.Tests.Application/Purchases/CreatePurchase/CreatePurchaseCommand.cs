using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Solace.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Solace.Tests.Application.Purchases.CreatePurchase
{
    public class CreatePurchaseCommand : IRequest<Guid>, ICommand
    {
        public CreatePurchaseCommand(Guid accountId, decimal amount)
        {
            AccountId = accountId;
            Amount = amount;
        }

        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
    }
}