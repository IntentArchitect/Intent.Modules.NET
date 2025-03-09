using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application.TypeTests
{
    public class BinaryTestDto
    {
        public BinaryTestDto()
        {
            BinaryField = null!;
            BinaryFieldCollection = null!;
        }

        public byte[] BinaryField { get; set; }
        public List<byte[]> BinaryFieldCollection { get; set; }
        public byte[]? BinaryFieldNullable { get; set; }
        public List<byte[]>? BinaryFieldNullableCollection { get; set; }

        public static BinaryTestDto Create(
            byte[] binaryField,
            List<byte[]> binaryFieldCollection,
            byte[]? binaryFieldNullable,
            List<byte[]>? binaryFieldNullableCollection)
        {
            return new BinaryTestDto
            {
                BinaryField = binaryField,
                BinaryFieldCollection = binaryFieldCollection,
                BinaryFieldNullable = binaryFieldNullable,
                BinaryFieldNullableCollection = binaryFieldNullableCollection
            };
        }
    }
}