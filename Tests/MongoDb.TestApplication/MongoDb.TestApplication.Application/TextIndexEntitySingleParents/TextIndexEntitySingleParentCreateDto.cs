using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.TextIndexEntitySingleParents
{
    public class TextIndexEntitySingleParentCreateDto
    {
        public TextIndexEntitySingleParentCreateDto()
        {
            SomeField = null!;
            TextIndexEntitySingleChild = null!;
        }

        public string SomeField { get; set; }
        public TextIndexEntitySingleChildDto TextIndexEntitySingleChild { get; set; }

        public static TextIndexEntitySingleParentCreateDto Create(
            string someField,
            TextIndexEntitySingleChildDto textIndexEntitySingleChild)
        {
            return new TextIndexEntitySingleParentCreateDto
            {
                SomeField = someField,
                TextIndexEntitySingleChild = textIndexEntitySingleChild
            };
        }
    }
}