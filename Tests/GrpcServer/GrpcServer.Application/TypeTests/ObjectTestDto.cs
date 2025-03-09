using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application.TypeTests
{
    public class ObjectTestDto
    {
        public ObjectTestDto()
        {
            ObjectField = null!;
            ObjectFieldCollection = null!;
        }

        public object ObjectField { get; set; }
        public List<object> ObjectFieldCollection { get; set; }
        public object? ObjectFieldNullable { get; set; }
        public List<object>? ObjectFieldNullableCollection { get; set; }

        public static ObjectTestDto Create(
            object objectField,
            List<object> objectFieldCollection,
            object? objectFieldNullable,
            List<object>? objectFieldNullableCollection)
        {
            return new ObjectTestDto
            {
                ObjectField = objectField,
                ObjectFieldCollection = objectFieldCollection,
                ObjectFieldNullable = objectFieldNullable,
                ObjectFieldNullableCollection = objectFieldNullableCollection
            };
        }
    }
}