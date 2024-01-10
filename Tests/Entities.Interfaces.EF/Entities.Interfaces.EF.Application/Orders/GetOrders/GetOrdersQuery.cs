using System.Collections.Generic;
using Entities.Interfaces.EF.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Entities.Interfaces.EF.Application.Orders.GetOrders
{
    public class GetOrdersQuery : IRequest<List<OrderDto>>, IQuery
    {
        public GetOrdersQuery()
        {
        }
    }
}