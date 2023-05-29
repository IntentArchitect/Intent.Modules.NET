using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MultikeyIndexEntities
{
    public class MultikeyIndexEntityCreateDto
    {
        public MultikeyIndexEntityCreateDto()
        {
            MultiKey = null!;
            SomeField = null!;
        }

        public IEnumerable<string> MultiKey { get; set; }
        public string SomeField { get; set; }

        public static MultikeyIndexEntityCreateDto Create(IEnumerable<string> multiKey, string someField)
        {
            return new MultikeyIndexEntityCreateDto
            {
                MultiKey = multiKey,
                SomeField = someField
            };
        }
    }
}