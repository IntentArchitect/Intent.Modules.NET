using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.ClassAS
{

    public class ClassAUpdateDTO
    {
        public ClassAUpdateDTO()
        {
            Attribute = null!;
        }

        public static ClassAUpdateDTO Create(Guid id, string attribute)
        {
            return new ClassAUpdateDTO
            {
                Id = id,
                Attribute = attribute
            };
        }

        public Guid Id { get; set; }

        public string Attribute { get; set; }

    }
}