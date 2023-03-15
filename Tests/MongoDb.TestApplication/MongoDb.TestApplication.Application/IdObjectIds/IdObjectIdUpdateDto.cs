using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdObjectIds
{

    public class IdObjectIdUpdateDto
    {
        public IdObjectIdUpdateDto()
        {
        }

        public static IdObjectIdUpdateDto Create(
            string id,
            string attribute)
        {
            return new IdObjectIdUpdateDto
            {
                Id = id,
                Attribute = attribute,
            };
        }

        public string Id { get; set; }

        public string Attribute { get; set; }

    }
}