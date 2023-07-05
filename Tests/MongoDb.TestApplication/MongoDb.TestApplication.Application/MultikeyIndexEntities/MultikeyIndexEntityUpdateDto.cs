using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MultikeyIndexEntities
{
    public class MultikeyIndexEntityUpdateDto
    {
        public MultikeyIndexEntityUpdateDto()
        {
            Id = null!;
            MultiKey = null!;
            SomeField = null!;
        }

        public string Id { get; set; }
        public List<string> MultiKey { get; set; }
        public string SomeField { get; set; }

        public static MultikeyIndexEntityUpdateDto Create(string id, List<string> multiKey, string someField)
        {
            return new MultikeyIndexEntityUpdateDto
            {
                Id = id,
                MultiKey = multiKey,
                SomeField = someField
            };
        }
    }
}