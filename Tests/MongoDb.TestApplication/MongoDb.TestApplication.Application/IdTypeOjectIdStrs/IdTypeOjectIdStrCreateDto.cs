using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.IdTypeOjectIdStrs
{
    public class IdTypeOjectIdStrCreateDto
    {
        public IdTypeOjectIdStrCreateDto()
        {
            Attribute = null!;
        }

        public string Attribute { get; set; }

        public static IdTypeOjectIdStrCreateDto Create(string attribute)
        {
            return new IdTypeOjectIdStrCreateDto
            {
                Attribute = attribute
            };
        }
    }
}