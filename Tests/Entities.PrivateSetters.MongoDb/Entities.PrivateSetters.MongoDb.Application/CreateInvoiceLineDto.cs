using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Application
{
    public class CreateInvoiceLineDto
    {
        public CreateInvoiceLineDto()
        {
        }

        public string Description { get; set; } = null!;
        public int Quantity { get; set; }

        public static CreateInvoiceLineDto Create(string description, int quantity)
        {
            return new CreateInvoiceLineDto
            {
                Description = description,
                Quantity = quantity
            };
        }
    }
}