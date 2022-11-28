using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeSingleAs
{

    public class UpdateCompositeSingleAADTO
    {
        public UpdateCompositeSingleAADTO()
        {
        }

        public static UpdateCompositeSingleAADTO Create(
            Guid id,
            string compositeAttr)
        {
            return new UpdateCompositeSingleAADTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }
    }
}