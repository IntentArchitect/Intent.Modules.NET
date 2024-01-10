using System;
using Entities.Interfaces.EF.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Entities.Interfaces.EF.Application.Orders.GetOrderById
{
    public class GetOrderByIdQuery : IRequest<OrderDto>, IQuery
    {
        public GetOrderByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}