using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.CompoundIndexEntitySingleParents
{
    public class CompoundIndexEntitySingleParentCreateDto
    {
        public CompoundIndexEntitySingleParentCreateDto()
        {
            SomeField = null!;
            CompoundIndexEntitySingleChild = null!;
        }

        public string SomeField { get; set; }
        public CompoundIndexEntitySingleChildDto CompoundIndexEntitySingleChild { get; set; }

        public static CompoundIndexEntitySingleParentCreateDto Create(
            string someField,
            CompoundIndexEntitySingleChildDto compoundIndexEntitySingleChild)
        {
            return new CompoundIndexEntitySingleParentCreateDto
            {
                SomeField = someField,
                CompoundIndexEntitySingleChild = compoundIndexEntitySingleChild
            };
        }
    }
}