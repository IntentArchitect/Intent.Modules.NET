using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace OpenApiImporterTest.Application.Customers
{
    public class CreateCustomerOrderCommand
    {
        public CreateCustomerOrderCommand()
        {
        }

        public Guid CustomerId { get; set; }
        public string? RefNo { get; set; }
        public decimal Price { get; set; }

        public static CreateCustomerOrderCommand Create(Guid customerId, string? refNo, decimal price)
        {
            return new CreateCustomerOrderCommand
            {
                CustomerId = customerId,
                RefNo = refNo,
                Price = price
            };
        }
    }
}