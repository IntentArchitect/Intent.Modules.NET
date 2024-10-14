using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Interfaces;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Orders;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Contracts;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Implementation
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
        public async Task<Guid> CreateOrder(OrderCreateDto dto, CancellationToken cancellationToken = default)
        {
            var order = new Order(
                refNo: dto.RefNo,
                orderDate: dto.OrderDate,
                orderItems: dto.OrderItems
                    .Select(i => new OrderItem(
                        productId: i.ProductId,
                        quantity: i.Quantity,
                        amount: i.Amount))
                    .ToList());

            _orderRepository.Add(order);
            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return order.Id;
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

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateOrderItems(Guid id, UpdateOrderItemsDto dto, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.FindByIdAsync(id, cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find Order '{id}'");
            }

            order.UpdateOrderItems(dto.OrderItemDetails
                .Select(id => new OrderItemUpdateDC(
                    id: id.Id,
                    amount: id.Amount,
                    quantity: id.Quantity,
                    productId: id.ProductId))
                .ToList());
        }

        public void Dispose()
        {
        }
    }
}