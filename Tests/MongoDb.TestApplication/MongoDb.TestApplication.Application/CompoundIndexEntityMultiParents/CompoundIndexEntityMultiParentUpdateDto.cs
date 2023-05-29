using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.CompoundIndexEntityMultiParents
{
    public class CompoundIndexEntityMultiParentUpdateDto
    {
        public CompoundIndexEntityMultiParentUpdateDto()
        {
            Id = null!;
            SomeField = null!;
        }

        public string Id { get; set; }
        public string SomeField { get; set; }

        public static CompoundIndexEntityMultiParentUpdateDto Create(string id, string someField)
        {
            return new CompoundIndexEntityMultiParentUpdateDto
            {
                Id = id,
                SomeField = someField
            };
        }
    }
}