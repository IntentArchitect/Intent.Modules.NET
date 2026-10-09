using AutoMapper;
using CqrsSpike.Subscriber.Domain.Common.Exceptions;
using CqrsSpike.Subscriber.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CqrsSpike.Subscriber.Application.OrderRecords.GetOrderRecordByOrderId
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetOrderRecordByOrderIdQueryHandler : IRequestHandler<GetOrderRecordByOrderIdQuery, OrderRecordDto>
    {
        private readonly IOrderRecordRepository _orderRecordRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetOrderRecordByOrderIdQueryHandler(IOrderRecordRepository orderRecordRepository, IMapper mapper)
        {
            _orderRecordRepository = orderRecordRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<OrderRecordDto> Handle(GetOrderRecordByOrderIdQuery request, CancellationToken cancellationToken)
        {
            var orderRecord = await _orderRecordRepository.FindAsync(x => x.OrderId == request.OrderId, cancellationToken);
            if (orderRecord is null)
            {
                throw new NotFoundException($"Could not find OrderRecord '{request.OrderId}'");
            }
            return orderRecord.MapToOrderRecordDto(_mapper);
        }
    }
}