using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdLongs
{

    public class IdLongCreateDto
    {
        public IdLongCreateDto()
        {
        }

        public static IdLongCreateDto Create(
            string attribute)
        {
            return new IdLongCreateDto
            {
                Attribute = attribute,
            };
        }

        public string Attribute { get; set; }

    }
}