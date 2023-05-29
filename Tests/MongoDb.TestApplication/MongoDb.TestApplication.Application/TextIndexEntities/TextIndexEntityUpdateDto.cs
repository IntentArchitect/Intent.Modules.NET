using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.TextIndexEntities
{
    public class TextIndexEntityUpdateDto
    {
        public TextIndexEntityUpdateDto()
        {
            Id = null!;
            FullText = null!;
            SomeField = null!;
        }

        public string Id { get; set; }
        public string FullText { get; set; }
        public string SomeField { get; set; }

        public static TextIndexEntityUpdateDto Create(string id, string fullText, string someField)
        {
            return new TextIndexEntityUpdateDto
            {
                Id = id,
                FullText = fullText,
                SomeField = someField
            };
        }
    }
}