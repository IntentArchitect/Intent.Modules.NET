using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdGuids
{

    public class IdGuidUpdateDto
    {
        public IdGuidUpdateDto()
        {
        }

        public static IdGuidUpdateDto Create(
            Guid id,
            string attribute)
        {
            return new IdGuidUpdateDto
            {
                Id = id,
                Attribute = attribute,
            };
        }

        public Guid Id { get; set; }

        public string Attribute { get; set; }

    }
}