using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace OpenApiImporterTest.Application.Customers
{
    public class UpdateCustomerOrderCommand
    {
        public UpdateCustomerOrderCommand()
        {
        }

        public Guid CustomerId { get; set; }
        public string? RefNo { get; set; }
        public decimal Price { get; set; }
        public Guid Id { get; set; }

        public static UpdateCustomerOrderCommand Create(Guid customerId, string? refNo, decimal price, Guid id)
        {
            return new UpdateCustomerOrderCommand
            {
                CustomerId = customerId,
                RefNo = refNo,
                Price = price,
                Id = id
            };
        }
    }
}