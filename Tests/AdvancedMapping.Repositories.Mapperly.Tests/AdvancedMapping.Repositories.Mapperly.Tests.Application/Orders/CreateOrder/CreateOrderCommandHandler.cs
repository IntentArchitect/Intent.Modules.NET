using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities.Sales;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Repositories.Sales;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders.CreateOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;

        [IntentManaged(Mode.Merge)]
        public CreateOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order
            {
                CustomerId = request.CustomerId,
                OrderDate = request.OrderDate,
                RequiredBy = request.RequiredBy,
                Status = request.Status,
                TotalAmount = request.TotalAmount,
                Shipments = request.Shipments
                    .Select(s => new Shipment
                    {
                        Provider = s.Provider,
                        ContainerId = s.ContainerId,
                        Dispatch = new Dispatch
                        {
                            OriginLocation = s.Dispatch.OriginLocation,
                            Document = new DispatchDocument
                            {
                                DocumentNumber = s.Dispatch.Document.DocumentNumber,
                                IssuedOn = s.Dispatch.Document.IssuedOn,
                                FileUrl = s.Dispatch.Document.FileUrl
                            }
                        },
                        Manifest = new Manifest
                        {
                            CarrierCode = s.Manifest.CarrierCode,
                            TotalWeight = s.Manifest.TotalWeight,
                            Document = new ManifestDocument
                            {
                                DocumentNumber = s.Manifest.Document.DocumentNumber,
                                IssuedOn = s.Manifest.Document.IssuedOn,
                                FileUrl = s.Manifest.Document.FileUrl
                            }
                        }
                    })
                    .ToList()
            };

            _orderRepository.Add(order);
            await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return order.Id;
        }
    }
}