using Intent.RoslynWeaver.Attributes;
using WolverineEventing.Subscribe.RabbitMQ.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.QueryHandler", Version = "1.0")]

namespace WolverineEventing.Subscribe.RabbitMQ.Application.GetShippedOrderRecords
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetShippedOrderRecordsQueryHandler
    {
        private readonly IShippedOrderRecordRepository _shippedOrderRecordRepository;

        [IntentManaged(Mode.Merge)]
        public GetShippedOrderRecordsQueryHandler(IShippedOrderRecordRepository shippedOrderRecordRepository)
        {
            _shippedOrderRecordRepository = shippedOrderRecordRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<ShippedOrderRecordDto>> Handle(
            GetShippedOrderRecordsQuery request,
            CancellationToken cancellationToken)
        {
            var records = await _shippedOrderRecordRepository.FindAllAsync(cancellationToken);
            return records.Select(r => new ShippedOrderRecordDto
            {
                OrderId = r.OrderId,
                ShippedAt = r.ShippedAt
            }).ToList();
        }
    }
}