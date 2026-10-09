using AutoMapper;
using CqrsSpike.Subscriber.Domain.Common.Exceptions;
using CqrsSpike.Subscriber.Domain.Repositories;
using Mediator;

namespace CqrsSpike.Subscriber.Application.OrderRecords.GetOrderRecordByOrderId
{
    public class GetOrderRecordByOrderIdQueryHandler : IQueryHandler<GetOrderRecordByOrderIdQuery, OrderRecordDto>
    {
        private readonly IOrderRecordRepository _orderRecordRepository;
        private readonly IMapper _mapper;

        public GetOrderRecordByOrderIdQueryHandler(IOrderRecordRepository orderRecordRepository, IMapper mapper)
        {
            _orderRecordRepository = orderRecordRepository;
            _mapper = mapper;
        }

        public async ValueTask<OrderRecordDto> Handle(GetOrderRecordByOrderIdQuery request, CancellationToken cancellationToken)
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