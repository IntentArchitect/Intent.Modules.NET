using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application.TypeTests
{
    public class BoolTestDto
    {
        public BoolTestDto()
        {
            BoolFieldCollection = null!;
        }

        public bool BoolField { get; set; }
        public List<bool> BoolFieldCollection { get; set; }
        public bool? BoolFieldNullable { get; set; }
        public List<bool>? BoolFieldNullableCollection { get; set; }

        public static BoolTestDto Create(
            bool boolField,
            List<bool> boolFieldCollection,
            bool? boolFieldNullable,
            List<bool>? boolFieldNullableCollection)
        {
            return new BoolTestDto
            {
                BoolField = boolField,
                BoolFieldCollection = boolFieldCollection,
                BoolFieldNullable = boolFieldNullable,
                BoolFieldNullableCollection = boolFieldNullableCollection
            };
        }
    }
}