using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.SingleIndexEntitySingleParents
{
    public class SingleIndexEntitySingleParentCreateDto
    {
        public SingleIndexEntitySingleParentCreateDto()
        {
            SomeField = null!;
            SingleIndexEntitySingleChild = null!;
        }

        public string SomeField { get; set; }
        public SingleIndexEntitySingleChildDto SingleIndexEntitySingleChild { get; set; }

        public static SingleIndexEntitySingleParentCreateDto Create(
            string someField,
            SingleIndexEntitySingleChildDto singleIndexEntitySingleChild)
        {
            return new SingleIndexEntitySingleParentCreateDto
            {
                SomeField = someField,
                SingleIndexEntitySingleChild = singleIndexEntitySingleChild
            };
        }
    }
}