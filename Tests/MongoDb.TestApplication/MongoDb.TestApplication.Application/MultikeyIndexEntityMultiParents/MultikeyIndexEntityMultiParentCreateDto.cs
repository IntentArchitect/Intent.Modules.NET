using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MultikeyIndexEntityMultiParents
{
    public class MultikeyIndexEntityMultiParentCreateDto
    {
        public MultikeyIndexEntityMultiParentCreateDto()
        {
            SomeField = null!;
            MultikeyIndexEntityMultiChild = null!;
        }

        public string SomeField { get; set; }
        public List<MultikeyIndexEntityMultiChildDto> MultikeyIndexEntityMultiChild { get; set; }

        public static MultikeyIndexEntityMultiParentCreateDto Create(
            string someField,
            List<MultikeyIndexEntityMultiChildDto> multikeyIndexEntityMultiChild)
        {
            return new MultikeyIndexEntityMultiParentCreateDto
            {
                SomeField = someField,
                MultikeyIndexEntityMultiChild = multikeyIndexEntityMultiChild
            };
        }
    }
}