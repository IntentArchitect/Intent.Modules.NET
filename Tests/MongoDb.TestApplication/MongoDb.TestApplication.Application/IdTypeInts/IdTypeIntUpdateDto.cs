using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdTypeInts
{
    public class IdTypeIntUpdateDto
    {
        public IdTypeIntUpdateDto()
        {
        }

        public int Id { get; set; }
        public string Attribute { get; set; }

        public static IdTypeIntUpdateDto Create(int id, string attribute)
        {
            return new IdTypeIntUpdateDto
            {
                Id = id,
                Attribute = attribute,
            };
        }
    }
}