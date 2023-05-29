using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MultikeyIndexEntitySingleParents
{
    public class MultikeyIndexEntitySingleParentCreateDto
    {
        public MultikeyIndexEntitySingleParentCreateDto()
        {
            SomeField = null!;
            MultikeyIndexEntitySingleChild = null!;
        }

        public string SomeField { get; set; }
        public MultikeyIndexEntitySingleChildDto MultikeyIndexEntitySingleChild { get; set; }

        public static MultikeyIndexEntitySingleParentCreateDto Create(
            string someField,
            MultikeyIndexEntitySingleChildDto multikeyIndexEntitySingleChild)
        {
            return new MultikeyIndexEntitySingleParentCreateDto
            {
                SomeField = someField,
                MultikeyIndexEntitySingleChild = multikeyIndexEntitySingleChild
            };
        }
    }
}