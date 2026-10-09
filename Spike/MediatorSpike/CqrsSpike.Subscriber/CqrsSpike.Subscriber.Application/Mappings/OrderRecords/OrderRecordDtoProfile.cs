using AutoMapper;
using CqrsSpike.Subscriber.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace CqrsSpike.Subscriber.Application.OrderRecords
{
    public class OrderRecordDtoProfile : Profile
    {
        public OrderRecordDtoProfile()
        {
            CreateMap<OrderRecord, OrderRecordDto>();
        }
    }

    public static class OrderRecordDtoMappingExtensions
    {
        public static OrderRecordDto MapToOrderRecordDto(this OrderRecord projectFrom, IMapper mapper) => mapper.Map<OrderRecordDto>(projectFrom);

        public static List<OrderRecordDto> MapToOrderRecordDtoList(
            this IEnumerable<OrderRecord> projectFrom,
            IMapper mapper) => projectFrom.Select(x => x.MapToOrderRecordDto(mapper)).ToList();
    }
}