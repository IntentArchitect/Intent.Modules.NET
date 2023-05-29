using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MultikeyIndexEntitySingleParents
{
    public class MultikeyIndexEntitySingleParentUpdateDto
    {
        public MultikeyIndexEntitySingleParentUpdateDto()
        {
            Id = null!;
            SomeField = null!;
        }

        public string Id { get; set; }
        public string SomeField { get; set; }

        public static MultikeyIndexEntitySingleParentUpdateDto Create(string id, string someField)
        {
            return new MultikeyIndexEntitySingleParentUpdateDto
            {
                Id = id,
                SomeField = someField
            };
        }
    }
}