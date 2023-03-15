using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdInts
{

    public class IdIntUpdateDto
    {
        public IdIntUpdateDto()
        {
        }

        public static IdIntUpdateDto Create(
            int id,
            string attribute)
        {
            return new IdIntUpdateDto
            {
                Id = id,
                Attribute = attribute,
            };
        }

        public int Id { get; set; }

        public string Attribute { get; set; }

    }
}