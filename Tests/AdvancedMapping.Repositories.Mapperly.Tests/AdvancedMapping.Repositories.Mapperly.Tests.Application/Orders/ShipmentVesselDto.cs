using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders
{
    public class ShipmentVesselDto
    {
        public ShipmentVesselDto()
        {
            DocumentNumber = null!;
        }

        public Guid Id { get; set; }
        public string DocumentNumber { get; set; }

        public static ShipmentVesselDto Create(Guid id, string documentNumber)
        {
            return new ShipmentVesselDto
            {
                Id = id,
                DocumentNumber = documentNumber
            };
        }
    }
}