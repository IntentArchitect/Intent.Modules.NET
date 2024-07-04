using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrudMongo.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrudMongo.Tests.Domain.Entities;
using AdvancedMappingCrudMongo.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Orders.GetOrderByRef
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOrderByRefQueryHandler : IRequestHandler<GetOrderByRefQuery, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetOrderByRefQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<OrderDto> Handle(GetOrderByRefQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Order> FilterOrders(IQueryable<Order> queryable)
            {
                if (request.RefNo != null)
                {
                    queryable = queryable.Where(x => x.RefNo == request.RefNo);
                }

                if (request.ExternalRefNo != null)
                {
                    queryable = queryable.Where(x => x.ExternalRef == request.ExternalRefNo);
                }
                return queryable;
            }

            var order = await _orderRepository.FindAsync(FilterOrders, cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find Order '({request.RefNo}, {request.ExternalRefNo})'");
            }
            return order.MapToOrderDto(_mapper);
        }
    }
}