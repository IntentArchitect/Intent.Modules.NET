using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application.TypeTests
{
    public class CharTestDto
    {
        public CharTestDto()
        {
            CharFieldCollection = null!;
        }

        public char CharField { get; set; }
        public List<char> CharFieldCollection { get; set; }
        public char? CharFieldNullable { get; set; }
        public List<char>? CharFieldNullableCollection { get; set; }

        public static CharTestDto Create(
            char charField,
            List<char> charFieldCollection,
            char? charFieldNullable,
            List<char>? charFieldNullableCollection)
        {
            return new CharTestDto
            {
                CharField = charField,
                CharFieldCollection = charFieldCollection,
                CharFieldNullable = charFieldNullable,
                CharFieldNullableCollection = charFieldNullableCollection
            };
        }
    }
}