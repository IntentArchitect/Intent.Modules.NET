using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdTypeGuids
{
    public class IdTypeGuidCreateDto
    {
        public IdTypeGuidCreateDto()
        {
        }

        public string Attribute { get; set; }

        public static IdTypeGuidCreateDto Create(string attribute)
        {
            return new IdTypeGuidCreateDto
            {
                Attribute = attribute,
            };
        }
    }
}