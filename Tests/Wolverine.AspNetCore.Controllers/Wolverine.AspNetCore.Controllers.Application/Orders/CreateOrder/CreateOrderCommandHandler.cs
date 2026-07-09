using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.Controllers.Domain;
using Wolverine.AspNetCore.Controllers.Domain.Entities;
using Wolverine.AspNetCore.Controllers.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandHandler", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Application.CreateOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOrderCommandHandler
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public CreateOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Merge, Signature = Mode.Fully, Body = Mode.Fully)]
        public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order
            {
                OrderNumber = request.OrderNumber,
                Status = request.Status,
                CustomerName = request.CustomerName,
                PlacedDate = request.PlacedDate,
                Notes = request.Notes,
                ShippingAddress = new ShippingAddress(
                    line1: request.ShippingLine1,
                    city: request.ShippingCity,
                    postalCode: request.ShippingPostalCode,
                    country: request.ShippingCountry)
            };

            _orderRepository.Add(order);
            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return order.MapToOrderDto(_mapper);
        }
    }
}