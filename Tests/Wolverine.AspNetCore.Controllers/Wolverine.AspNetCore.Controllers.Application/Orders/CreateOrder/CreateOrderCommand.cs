using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.Controllers.Application.Common.Interfaces;
using Wolverine.AspNetCore.Controllers.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.CommandModels", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Application.CreateOrder
{
    public class CreateOrderCommand : ICommand
    {
        public CreateOrderCommand(string orderNumber,
            string customerName,
            OrderStatus status,
            DateTime placedDate,
            string? notes,
            string shippingLine1,
            string shippingCity,
            string shippingPostalCode,
            string shippingCountry)
        {
            OrderNumber = orderNumber;
            CustomerName = customerName;
            Status = status;
            PlacedDate = placedDate;
            Notes = notes;
            ShippingLine1 = shippingLine1;
            ShippingCity = shippingCity;
            ShippingPostalCode = shippingPostalCode;
            ShippingCountry = shippingCountry;
        }

        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime PlacedDate { get; set; }
        public string? Notes { get; set; }
        public string ShippingLine1 { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingPostalCode { get; set; }
        public string ShippingCountry { get; set; }
    }
}