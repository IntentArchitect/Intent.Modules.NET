using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Application
{
    public class CreateInvoiceDto
    {
        public CreateInvoiceDto()
        {
            TagIds = null!;
            Lines = null!;
        }

        public DateTime Date { get; set; }
        public List<string> TagIds { get; set; }
        public List<CreateInvoiceLineDto> Lines { get; set; }

        public static CreateInvoiceDto Create(DateTime date, List<string> tagIds, List<CreateInvoiceLineDto> lines)
        {
            return new CreateInvoiceDto
            {
                Date = date,
                TagIds = tagIds,
                Lines = lines
            };
        }
    }
}