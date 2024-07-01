using Intent.RoslynWeaver.Attributes;
using MediatR;
using OpenApiImporterTest.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace OpenApiImporterTest.Application.Stores.CreateOrder
{
    public class CreateOrderCommand : IRequest<Order>, ICommand
    {
        public CreateOrderCommand(int id, int petId, int quantity, string shipDate, StatusType status, bool complete)
        {
            Id = id;
            PetId = petId;
            Quantity = quantity;
            ShipDate = shipDate;
            Status = status;
            Complete = complete;
        }

        public int Id { get; set; }
        public int PetId { get; set; }
        public int Quantity { get; set; }
        public string ShipDate { get; set; }
        public StatusType Status { get; set; }
        public bool Complete { get; set; }
    }
}