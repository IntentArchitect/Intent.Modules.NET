using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.TextIndexEntityMultiParents
{
    public class TextIndexEntityMultiParentUpdateDto
    {
        public TextIndexEntityMultiParentUpdateDto()
        {
            Id = null!;
            SomeField = null!;
        }

        public string Id { get; set; }
        public string SomeField { get; set; }

        public static TextIndexEntityMultiParentUpdateDto Create(string id, string someField)
        {
            return new TextIndexEntityMultiParentUpdateDto
            {
                Id = id,
                SomeField = someField
            };
        }
    }
}