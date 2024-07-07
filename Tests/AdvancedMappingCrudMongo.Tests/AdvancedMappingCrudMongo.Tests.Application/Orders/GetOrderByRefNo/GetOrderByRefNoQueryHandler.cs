using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrudMongo.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrudMongo.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Orders.GetOrderByRefNo
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOrderByRefNoQueryHandler : IRequestHandler<GetOrderByRefNoQuery, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetOrderByRefNoQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<OrderDto> Handle(GetOrderByRefNoQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.FindAsync(x => x.RefNo == request.RefNo && x.ExternalRef == request.External, cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find Order '({request.RefNo}, {request.External})'");
            }
            return order.MapToOrderDto(_mapper);
        }
    }
}