using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MultikeyIndexEntityMultiParents
{
    public class MultikeyIndexEntityMultiParentUpdateDto
    {
        public MultikeyIndexEntityMultiParentUpdateDto()
        {
            Id = null!;
            SomeField = null!;
        }

        public string Id { get; set; }
        public string SomeField { get; set; }

        public static MultikeyIndexEntityMultiParentUpdateDto Create(string id, string someField)
        {
            return new MultikeyIndexEntityMultiParentUpdateDto
            {
                Id = id,
                SomeField = someField
            };
        }
    }
}