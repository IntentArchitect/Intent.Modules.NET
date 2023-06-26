using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.GetBasketBasketItems
{
    public class GetBasketBasketItemsQuery : IRequest<List<BasketBasketItemDto>>, IQuery
    {
        public GetBasketBasketItemsQuery(Guid basketId)
        {
            BasketId = basketId;
        }

        public Guid BasketId { get; set; }
    }
}