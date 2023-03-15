using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdObjectIds
{

    public class IdObjectIdCreateDto
    {
        public IdObjectIdCreateDto()
        {
        }

        public static IdObjectIdCreateDto Create(
            string attribute)
        {
            return new IdObjectIdCreateDto
            {
                Attribute = attribute,
            };
        }

        public string Attribute { get; set; }

    }
}