using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdLongs
{

    public class IdLongUpdateDto
    {
        public IdLongUpdateDto()
        {
        }

        public static IdLongUpdateDto Create(
            long id,
            string attribute)
        {
            return new IdLongUpdateDto
            {
                Id = id,
                Attribute = attribute,
            };
        }

        public long Id { get; set; }

        public string Attribute { get; set; }

    }
}