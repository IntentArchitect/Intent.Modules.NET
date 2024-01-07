using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Pagination;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Orders.GetOrdersPaginated
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOrdersPaginatedQueryHandler : IRequestHandler<GetOrdersPaginatedQuery, PagedResult<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetOrdersPaginatedQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PagedResult<OrderDto>> Handle(
            GetOrdersPaginatedQuery request,
            CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.FindAllAsync(request.PageNo, request.PageSize, cancellationToken);
            return orders.MapToPagedResult(x => x.MapToOrderDto(_mapper));
        }
    }
}