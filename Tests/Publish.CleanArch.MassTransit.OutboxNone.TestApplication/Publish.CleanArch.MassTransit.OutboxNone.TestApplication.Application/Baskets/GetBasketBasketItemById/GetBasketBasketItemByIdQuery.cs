using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.GetBasketBasketItemById
{
    public class GetBasketBasketItemByIdQuery : IRequest<BasketBasketItemDto>, IQuery
    {
        public GetBasketBasketItemByIdQuery(Guid basketId, Guid id)
        {
            BasketId = basketId;
            Id = id;
        }

        public Guid BasketId { get; set; }
        public Guid Id { get; set; }
    }
}