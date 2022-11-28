using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeSingleAs
{

    public class CreateCompositeSingleAADTO
    {
        public CreateCompositeSingleAADTO()
        {
        }

        public static CreateCompositeSingleAADTO Create(
            Guid id,
            string compositeAttr)
        {
            return new CreateCompositeSingleAADTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }
    }
}