using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdGuids
{

    public class IdGuidCreateDto
    {
        public IdGuidCreateDto()
        {
        }

        public static IdGuidCreateDto Create(
            string attribute)
        {
            return new IdGuidCreateDto
            {
                Attribute = attribute,
            };
        }

        public string Attribute { get; set; }

    }
}