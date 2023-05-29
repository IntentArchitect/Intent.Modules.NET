using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.CompoundIndexEntityMultiParents
{
    public class CompoundIndexEntityMultiParentCreateDto
    {
        public CompoundIndexEntityMultiParentCreateDto()
        {
            SomeField = null!;
            CompoundIndexEntityMultiChild = null!;
        }

        public string SomeField { get; set; }
        public List<CompoundIndexEntityMultiChildDto> CompoundIndexEntityMultiChild { get; set; }

        public static CompoundIndexEntityMultiParentCreateDto Create(
            string someField,
            List<CompoundIndexEntityMultiChildDto> compoundIndexEntityMultiChild)
        {
            return new CompoundIndexEntityMultiParentCreateDto
            {
                SomeField = someField,
                CompoundIndexEntityMultiChild = compoundIndexEntityMultiChild
            };
        }
    }
}