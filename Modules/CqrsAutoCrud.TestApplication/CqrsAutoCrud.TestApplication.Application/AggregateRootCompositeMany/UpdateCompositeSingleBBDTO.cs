using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeMany
{

    public class UpdateCompositeSingleBBDTO
    {
        public UpdateCompositeSingleBBDTO()
        {
        }

        public static UpdateCompositeSingleBBDTO Create(
            Guid id,
            string compositeAttr)
        {
            return new UpdateCompositeSingleBBDTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }
    }
}