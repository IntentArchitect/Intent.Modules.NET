using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace OpenApiImporterTest.Application.Stores
{
    public class Order
    {
        public Order()
        {
            ShipDate = null!;
        }

        public int Id { get; set; }
        public int PetId { get; set; }
        public int Quantity { get; set; }
        public string ShipDate { get; set; }
        public StatusType Status { get; set; }
        public bool Complete { get; set; }

        public static Order Create(int id, int petId, int quantity, string shipDate, StatusType status, bool complete)
        {
            return new Order
            {
                Id = id,
                PetId = petId,
                Quantity = quantity,
                ShipDate = shipDate,
                Status = status,
                Complete = complete
            };
        }
    }
}