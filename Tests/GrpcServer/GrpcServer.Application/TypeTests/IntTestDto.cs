using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application.TypeTests
{
    public class IntTestDto
    {
        public IntTestDto()
        {
            IntFieldCollection = null!;
        }

        public int IntField { get; set; }
        public List<int> IntFieldCollection { get; set; }
        public int? IntFieldNullable { get; set; }
        public List<int>? IntFieldNullableCollection { get; set; }

        public static IntTestDto Create(
            int intField,
            List<int> intFieldCollection,
            int? intFieldNullable,
            List<int>? intFieldNullableCollection)
        {
            return new IntTestDto
            {
                IntField = intField,
                IntFieldCollection = intFieldCollection,
                IntFieldNullable = intFieldNullable,
                IntFieldNullableCollection = intFieldNullableCollection
            };
        }
    }
}