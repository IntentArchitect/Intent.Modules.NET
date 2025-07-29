using AutoMapper;
using EntityFrameworkCore.SQLLite.Application.Interfaces;
using EntityFrameworkCore.SQLLite.Application.Orders;
using EntityFrameworkCore.SQLLite.Domain.Common.Exceptions;
using EntityFrameworkCore.SQLLite.Domain.Entities;
using EntityFrameworkCore.SQLLite.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace EntityFrameworkCore.SQLLite.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class OrdersService : IOrdersService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public OrdersService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateOrder(CreateOrderDto dto, CancellationToken cancellationToken = default)
        {
            var order = new Order
            {
                RefNo = dto.RefNo
            };

            _orderRepository.Add(order);
            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return order.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateOrder(Guid id, UpdateOrderDto dto, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.FindByIdAsync(id, cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find Order '{id}'");
            }

            order.RefNo = dto.RefNo;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<OrderDto> FindOrderById(Guid id, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.FindByIdAsync(id, cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find Order '{id}'");
            }
            return order.MapToOrderDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<OrderDto>> FindOrders(CancellationToken cancellationToken = default)
        {
            var orders = await _orderRepository.FindAllAsync(cancellationToken);
            return orders.MapToOrderDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteOrder(Guid id, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.FindByIdAsync(id, cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find Order '{id}'");
            }


            _orderRepository.Remove(order);
        }
    }
}