using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application.TypeTests
{
    public class StringTestDto
    {
        public StringTestDto()
        {
            StringField = null!;
            StringFieldCollection = null!;
        }

        public string StringField { get; set; }
        public List<string> StringFieldCollection { get; set; }
        public string? StringFieldNullable { get; set; }
        public List<string>? StringFieldNullableCollection { get; set; }

        public static StringTestDto Create(
            string stringField,
            List<string> stringFieldCollection,
            string? stringFieldNullable,
            List<string>? stringFieldNullableCollection)
        {
            return new StringTestDto
            {
                StringField = stringField,
                StringFieldCollection = stringFieldCollection,
                StringFieldNullable = stringFieldNullable,
                StringFieldNullableCollection = stringFieldNullableCollection
            };
        }
    }
}