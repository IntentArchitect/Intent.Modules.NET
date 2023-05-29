using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.SingleIndexEntitySingleParents
{
    public class SingleIndexEntitySingleParentUpdateDto
    {
        public SingleIndexEntitySingleParentUpdateDto()
        {
            Id = null!;
            SomeField = null!;
        }

        public string Id { get; set; }
        public string SomeField { get; set; }

        public static SingleIndexEntitySingleParentUpdateDto Create(string id, string someField)
        {
            return new SingleIndexEntitySingleParentUpdateDto
            {
                Id = id,
                SomeField = someField
            };
        }
    }
}