using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Publish.CleanArch.MassTransit.OutboxNone.TestApplication.Application.Baskets.GetBasketById
{
    public class GetBasketByIdQuery : IRequest<BasketDto>, IQuery
    {
        public GetBasketByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}