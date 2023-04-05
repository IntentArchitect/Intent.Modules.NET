using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdTypeLongs
{
    public class IdTypeLongUpdateDto
    {
        public IdTypeLongUpdateDto()
        {
        }

        public long Id { get; set; }
        public string Attribute { get; set; }

        public static IdTypeLongUpdateDto Create(long id, string attribute)
        {
            return new IdTypeLongUpdateDto
            {
                Id = id,
                Attribute = attribute,
            };
        }
    }
}