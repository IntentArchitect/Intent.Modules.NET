using CqrsSpike.Subscriber.Domain.Entities;
using CqrsSpike.Subscriber.Domain.Repositories;
using Mediator;

namespace CqrsSpike.Subscriber.Application.OrderRecords.RecordOrder
{
    public class RecordOrderCommandHandler : ICommandHandler<RecordOrderCommand>
    {
        private readonly IOrderRecordRepository _orderRecordRepository;

        public RecordOrderCommandHandler(IOrderRecordRepository orderRecordRepository)
        {
            _orderRecordRepository = orderRecordRepository;
        }

        public async ValueTask<Unit> Handle(RecordOrderCommand request, CancellationToken cancellationToken)
        {
            // OrderCreatedEvent is delivered at least once; a redelivery must not record the order twice.
            if (await _orderRecordRepository.AnyAsync(x => x.OrderId == request.OrderId, cancellationToken))
            {
                return Unit.Value;
            }

            var orderRecord = new OrderRecord
            {
                CustomerName = request.CustomerName,
                Total = request.Total,
                OrderId = request.OrderId
            };

            _orderRecordRepository.Add(orderRecord);
            return Unit.Value;
        }
    }
}