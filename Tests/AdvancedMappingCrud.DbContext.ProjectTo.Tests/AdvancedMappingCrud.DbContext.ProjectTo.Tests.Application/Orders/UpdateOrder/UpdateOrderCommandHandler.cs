using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.DbContext.ProjectTo.Tests.Domain.Common.Exceptions;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.DbContext.ProjectTo.Tests.Application.Orders.UpdateOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand>
    {
        private readonly IApplicationDbContext _dbContext;

        [IntentManaged(Mode.Merge)]
        public UpdateOrderCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _dbContext.Orders.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Could not find Order '{request.Id}'");
            }

            order.RefNo = request.RefNo;
            order.OrderDate = request.OrderDate;
            order.OrderStatus = request.OrderStatus;
            order.CustomerId = request.CustomerId;
        }
    }
}