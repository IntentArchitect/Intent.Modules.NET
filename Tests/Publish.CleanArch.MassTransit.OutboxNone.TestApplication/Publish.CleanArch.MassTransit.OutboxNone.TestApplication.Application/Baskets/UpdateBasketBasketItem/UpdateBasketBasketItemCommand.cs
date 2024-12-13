using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.UpdateBasketBasketItem
{
    public class UpdateBasketBasketItemCommand : IRequest, ICommand
    {
        public UpdateBasketBasketItemCommand(Guid id, string description, decimal amount, Guid basketId)
        {
            Id = id;
            Description = description;
            Amount = amount;
            BasketId = basketId;
        }

        public Guid BasketId { get; set; }
        public Guid Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }
}