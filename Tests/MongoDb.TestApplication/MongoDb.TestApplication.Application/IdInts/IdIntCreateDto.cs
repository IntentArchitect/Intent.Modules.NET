using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdInts
{

    public class IdIntCreateDto
    {
        public IdIntCreateDto()
        {
        }

        public static IdIntCreateDto Create(
            string attribute)
        {
            return new IdIntCreateDto
            {
                Attribute = attribute,
            };
        }

        public string Attribute { get; set; }

    }
}