using AutoMapper;
using Intent.Modules.NET.Tests.Domain.Core.Common.Exceptions;
using Intent.Modules.NET.Tests.Module1.Application.Contracts.Interfaces;
using Intent.Modules.NET.Tests.Module1.Application.Contracts.Orders;
using Intent.Modules.NET.Tests.Module1.Application.Orders;
using Intent.Modules.NET.Tests.Module1.Domain.Common.Interfaces;
using Intent.Modules.NET.Tests.Module1.Domain.Entities;
using Intent.Modules.NET.Tests.Module1.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module1.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class OrdersService : IOrdersService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        [IntentManaged(Mode.Merge)]
        public OrdersService(IOrderRepository orderRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateOrder(OrderCreateDto dto, CancellationToken cancellationToken = default)
        {
            var order = new Order
            {
                CustomerId = dto.CustomerId,
                RefNo = dto.RefNo,
                OrderItems = dto.OrderItems
                    .Select(oi => new OrderItem
                    {
                        Quantiity = oi.Quantiity,
                        Amount = oi.Amount
                    })
                    .ToList()
            };

            _orderRepository.Add(order);
            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return order.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateOrder(Guid id, OrderUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.FindByIdAsync(id, cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find Order '{id}'");
            }

            order.CustomerId = dto.CustomerId;
            order.RefNo = dto.RefNo;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
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

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}