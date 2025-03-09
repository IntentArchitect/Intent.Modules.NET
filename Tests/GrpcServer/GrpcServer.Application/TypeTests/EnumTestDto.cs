using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application.TypeTests
{
    public class EnumTestDto
    {
        public EnumTestDto()
        {
            EnumFieldCollection = null!;
        }

        public EnumType EnumField { get; set; }
        public List<EnumType> EnumFieldCollection { get; set; }
        public EnumType? EnumFieldNullable { get; set; }
        public List<EnumType>? EnumFieldNullableCollection { get; set; }

        public static EnumTestDto Create(
            EnumType enumField,
            List<EnumType> enumFieldCollection,
            EnumType? enumFieldNullable,
            List<EnumType>? enumFieldNullableCollection)
        {
            return new EnumTestDto
            {
                EnumField = enumField,
                EnumFieldCollection = enumFieldCollection,
                EnumFieldNullable = enumFieldNullable,
                EnumFieldNullableCollection = enumFieldNullableCollection
            };
        }
    }
}