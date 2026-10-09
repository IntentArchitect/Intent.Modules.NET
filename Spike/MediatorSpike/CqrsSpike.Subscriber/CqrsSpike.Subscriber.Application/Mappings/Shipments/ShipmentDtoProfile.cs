using AutoMapper;
using CqrsSpike.Subscriber.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace CqrsSpike.Subscriber.Application.Shipments
{
    public class ShipmentDtoProfile : Profile
    {
        public ShipmentDtoProfile()
        {
            CreateMap<Shipment, ShipmentDto>();
        }
    }

    public static class ShipmentDtoMappingExtensions
    {
        public static ShipmentDto MapToShipmentDto(this Shipment projectFrom, IMapper mapper) => mapper.Map<ShipmentDto>(projectFrom);

        public static List<ShipmentDto> MapToShipmentDtoList(this IEnumerable<Shipment> projectFrom, IMapper mapper) => projectFrom.Select(x => x.MapToShipmentDto(mapper)).ToList();
    }
}