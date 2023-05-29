using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.TextIndexEntityMultiParents
{
    public class TextIndexEntityMultiParentCreateDto
    {
        public TextIndexEntityMultiParentCreateDto()
        {
            SomeField = null!;
            TextIndexEntityMultiChild = null!;
        }

        public string SomeField { get; set; }
        public List<TextIndexEntityMultiChildDto> TextIndexEntityMultiChild { get; set; }

        public static TextIndexEntityMultiParentCreateDto Create(
            string someField,
            List<TextIndexEntityMultiChildDto> textIndexEntityMultiChild)
        {
            return new TextIndexEntityMultiParentCreateDto
            {
                SomeField = someField,
                TextIndexEntityMultiChild = textIndexEntityMultiChild
            };
        }
    }
}