using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.ClassAS
{

    public class ClassACreateDTO
    {
        public ClassACreateDTO()
        {
        }

        public static ClassACreateDTO Create(
            string attribute)
        {
            return new ClassACreateDTO
            {
                Attribute = attribute,
            };
        }

        public string Attribute { get; set; }

    }
}