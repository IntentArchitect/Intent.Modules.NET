using AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Riok.Mapperly.Abstractions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.Mapperly.DtoMappingProfile", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Mappings.Orders
{
    [Mapper]
    public partial class ShipmentVesselDtoMapper
    {
        [MapperIgnoreSource(nameof(Vessel.ContainerId))]
        [MapProperty(nameof(Vessel.IMOCode), nameof(ShipmentVesselDto.DocumentNumber))]
        public partial ShipmentVesselDto VesselToShipmentVesselDto(Vessel vessel);

        public partial List<ShipmentVesselDto> VesselToShipmentVesselDtoList(IEnumerable<Vessel> vessels);
    }
}