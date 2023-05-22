using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.PrivateSetters.EF.CosmosDb.Application
{
    public class CreateInvoiceLineDto
    {
        public CreateInvoiceLineDto()
        {
            Description = null!;
        }

        public string Description { get; set; }
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