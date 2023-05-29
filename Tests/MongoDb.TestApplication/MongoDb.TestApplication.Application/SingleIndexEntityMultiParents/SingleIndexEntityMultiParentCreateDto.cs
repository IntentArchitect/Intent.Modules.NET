using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.SingleIndexEntityMultiParents
{
    public class SingleIndexEntityMultiParentCreateDto
    {
        public SingleIndexEntityMultiParentCreateDto()
        {
            SomeField = null!;
            SingleIndexEntityMultiChild = null!;
        }

        public string SomeField { get; set; }
        public List<SingleIndexEntityMultiChildDto> SingleIndexEntityMultiChild { get; set; }

        public static SingleIndexEntityMultiParentCreateDto Create(
            string someField,
            List<SingleIndexEntityMultiChildDto> singleIndexEntityMultiChild)
        {
            return new SingleIndexEntityMultiParentCreateDto
            {
                SomeField = someField,
                SingleIndexEntityMultiChild = singleIndexEntityMultiChild
            };
        }
    }
}