using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application.TypeTests
{
    public class FloatTestDto
    {
        public FloatTestDto()
        {
            FloatFieldCollection = null!;
        }

        public float FloatField { get; set; }
        public List<float> FloatFieldCollection { get; set; }
        public float? FloatFieldNullable { get; set; }
        public List<float>? FloatFieldNullableCollection { get; set; }

        public static FloatTestDto Create(
            float floatField,
            List<float> floatFieldCollection,
            float? floatFieldNullable,
            List<float>? floatFieldNullableCollection)
        {
            return new FloatTestDto
            {
                FloatField = floatField,
                FloatFieldCollection = floatFieldCollection,
                FloatFieldNullable = floatFieldNullable,
                FloatFieldNullableCollection = floatFieldNullableCollection
            };
        }
    }
}