using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.DbContext.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.DbContext.Tests.Domain.Common.Exceptions;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.Orders.UpdateOrderOrderItem
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateOrderOrderItemCommandHandler : IRequestHandler<UpdateOrderOrderItemCommand>
    {
        private readonly IApplicationDbContext _dbContext;

        [IntentManaged(Mode.Merge)]
        public UpdateOrderOrderItemCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateOrderOrderItemCommand request, CancellationToken cancellationToken)
        {
            var order = await _dbContext.Orders.SingleOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find OrderItem '{request.OrderId}'");
            }

            var orderItem = order.OrderItems.FirstOrDefault(x => x.Id == request.Id);
            if (orderItem is null)
            {
                throw new NotFoundException($"Could not find OrderItem '{request.Id}'");
            }

            orderItem.OrderId = request.OrderId;
            orderItem.Quantity = request.Quantity;
            orderItem.Amount = request.Amount;
            orderItem.ProductId = request.ProductId;
        }
    }
}