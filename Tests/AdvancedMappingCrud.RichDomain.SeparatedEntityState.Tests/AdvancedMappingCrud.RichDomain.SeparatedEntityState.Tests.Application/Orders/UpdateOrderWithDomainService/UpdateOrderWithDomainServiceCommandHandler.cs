using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Contracts;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Services;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Orders.UpdateOrderWithDomainService
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateOrderWithDomainServiceCommandHandler : IRequestHandler<UpdateOrderWithDomainServiceCommand>
    {
        private readonly IOrderDomainService _orderDomainService;

        [IntentManaged(Mode.Merge)]
        public UpdateOrderWithDomainServiceCommandHandler(IOrderDomainService orderDomainService)
        {
            _orderDomainService = orderDomainService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateOrderWithDomainServiceCommand request, CancellationToken cancellationToken)
        {
            _orderDomainService.UpdateLineItems(request.Id, request.OrderItemDetails
                .Select(li => new OrderItemUpdateDC(
                    id: li.Id,
                    amount: li.Amount,
                    quantity: li.Quantity,
                    productId: li.ProductId))
                .ToList());
        }
    }
}