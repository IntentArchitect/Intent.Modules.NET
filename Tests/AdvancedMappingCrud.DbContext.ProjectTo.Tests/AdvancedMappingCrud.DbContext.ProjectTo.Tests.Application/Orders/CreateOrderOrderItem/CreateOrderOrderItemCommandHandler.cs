using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.ProjectTo.Tests.Application.Orders.CreateOrderOrderItem
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOrderOrderItemCommandHandler : IRequestHandler<CreateOrderOrderItemCommand, Guid>
    {
        private readonly IApplicationDbContext _dbContext;

        [IntentManaged(Mode.Merge)]
        public CreateOrderOrderItemCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateOrderOrderItemCommand request, CancellationToken cancellationToken)
        {
            var order = await _dbContext.Orders.SingleOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find Order '{request.OrderId}'");
            }
            var orderItem = new OrderItem
            {
                OrderId = request.OrderId,
                Quantity = request.Quantity,
                Amount = request.Amount,
                ProductId = request.ProductId
            };

            order.OrderItems.Add(orderItem);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return orderItem.Id;
        }
    }
}