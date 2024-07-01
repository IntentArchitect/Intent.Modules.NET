using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace OpenApiImporterTest.Application.Customers
{
    public class CustomerOrderDto
    {
        public CustomerOrderDto()
        {
            Customer = null!;
        }

        public Guid CustomerId { get; set; }
        public Guid Id { get; set; }
        public string? RefNo { get; set; }
        public decimal Price { get; set; }
        public CustomerOrderCustomerDto Customer { get; set; }

        public static CustomerOrderDto Create(
            Guid customerId,
            Guid id,
            string? refNo,
            decimal price,
            CustomerOrderCustomerDto customer)
        {
            return new CustomerOrderDto
            {
                CustomerId = customerId,
                Id = id,
                RefNo = refNo,
                Price = price,
                Customer = customer
            };
        }
    }
}