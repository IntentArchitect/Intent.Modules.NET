using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdTypeLongs
{
    public class IdTypeLongCreateDto
    {
        public IdTypeLongCreateDto()
        {
        }

        public string Attribute { get; set; }

        public static IdTypeLongCreateDto Create(string attribute)
        {
            return new IdTypeLongCreateDto
            {
                Attribute = attribute,
            };
        }
    }
}