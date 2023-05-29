using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.SingleIndexEntityMultiParents
{
    public class SingleIndexEntityMultiParentUpdateDto
    {
        public SingleIndexEntityMultiParentUpdateDto()
        {
            Id = null!;
            SomeField = null!;
        }

        public string Id { get; set; }
        public string SomeField { get; set; }

        public static SingleIndexEntityMultiParentUpdateDto Create(string id, string someField)
        {
            return new SingleIndexEntityMultiParentUpdateDto
            {
                Id = id,
                SomeField = someField
            };
        }
    }
}