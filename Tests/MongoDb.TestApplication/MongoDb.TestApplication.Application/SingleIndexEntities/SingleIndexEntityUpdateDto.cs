using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.SingleIndexEntities
{
    public class SingleIndexEntityUpdateDto
    {
        public SingleIndexEntityUpdateDto()
        {
            Id = null!;
            SomeField = null!;
            SingleIndex = null!;
        }

        public string Id { get; set; }
        public string SomeField { get; set; }
        public string SingleIndex { get; set; }

        public static SingleIndexEntityUpdateDto Create(string id, string someField, string singleIndex)
        {
            return new SingleIndexEntityUpdateDto
            {
                Id = id,
                SomeField = someField,
                SingleIndex = singleIndex
            };
        }
    }
}