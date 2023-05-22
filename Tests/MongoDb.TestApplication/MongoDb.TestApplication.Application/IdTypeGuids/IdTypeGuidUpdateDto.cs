using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdTypeGuids
{
    public class IdTypeGuidUpdateDto
    {
        public IdTypeGuidUpdateDto()
        {
        }

        public Guid Id { get; set; }
        public string Attribute { get; set; } = null!;

        public static IdTypeGuidUpdateDto Create(Guid id, string attribute)
        {
            return new IdTypeGuidUpdateDto
            {
                Id = id,
                Attribute = attribute
            };
        }
    }
}