using CqrsSpike.Subscriber.Domain.Entities;
using CqrsSpike.Subscriber.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CqrsSpike.Subscriber.Application.OrderRecords.RecordOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class RecordOrderCommandHandler : IRequestHandler<RecordOrderCommand>
    {
        private readonly IOrderRecordRepository _orderRecordRepository;

        [IntentManaged(Mode.Merge)]
        public RecordOrderCommandHandler(IOrderRecordRepository orderRecordRepository)
        {
            _orderRecordRepository = orderRecordRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(RecordOrderCommand request, CancellationToken cancellationToken)
        {
            var orderRecord = new OrderRecord
            {
                CustomerName = request.CustomerName,
                Total = request.Total,
                OrderId = request.OrderId
            };

            _orderRecordRepository.Add(orderRecord);
        }
    }
}