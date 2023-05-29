using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.CompoundIndexEntitySingleParents
{
    public class CompoundIndexEntitySingleParentUpdateDto
    {
        public CompoundIndexEntitySingleParentUpdateDto()
        {
            Id = null!;
            SomeField = null!;
        }

        public string Id { get; set; }
        public string SomeField { get; set; }

        public static CompoundIndexEntitySingleParentUpdateDto Create(string id, string someField)
        {
            return new CompoundIndexEntitySingleParentUpdateDto
            {
                Id = id,
                SomeField = someField
            };
        }
    }
}