using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.TextIndexEntities
{
    public class TextIndexEntityCreateDto
    {
        public TextIndexEntityCreateDto()
        {
            FullText = null!;
            SomeField = null!;
        }

        public string FullText { get; set; }
        public string SomeField { get; set; }

        public static TextIndexEntityCreateDto Create(string fullText, string someField)
        {
            return new TextIndexEntityCreateDto
            {
                FullText = fullText,
                SomeField = someField
            };
        }
    }
}