using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.SingleIndexEntities
{
    public class SingleIndexEntityCreateDto
    {
        public SingleIndexEntityCreateDto()
        {
            SomeField = null!;
            SingleIndex = null!;
        }

        public string SomeField { get; set; }
        public string SingleIndex { get; set; }

        public static SingleIndexEntityCreateDto Create(string someField, string singleIndex)
        {
            return new SingleIndexEntityCreateDto
            {
                SomeField = someField,
                SingleIndex = singleIndex
            };
        }
    }
}