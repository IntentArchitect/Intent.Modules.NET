using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.PagingTS
{
    public class PagingTSUpdateDto
    {
        public PagingTSUpdateDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static PagingTSUpdateDto Create(Guid id, string name)
        {
            return new PagingTSUpdateDto
            {
                Id = id,
                Name = name
            };
        }
    }
}