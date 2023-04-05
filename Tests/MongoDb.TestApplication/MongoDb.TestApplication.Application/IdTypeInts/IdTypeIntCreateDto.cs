using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdTypeInts
{
    public class IdTypeIntCreateDto
    {
        public IdTypeIntCreateDto()
        {
        }

        public string Attribute { get; set; }

        public static IdTypeIntCreateDto Create(string attribute)
        {
            return new IdTypeIntCreateDto
            {
                Attribute = attribute,
            };
        }
    }
}