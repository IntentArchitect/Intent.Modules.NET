using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application.TypeTests
{
    public class ByteTestDto
    {
        public ByteTestDto()
        {
            ByteFieldCollection = null!;
        }

        public byte ByteField { get; set; }
        public List<byte> ByteFieldCollection { get; set; }
        public byte? ByteFieldNullable { get; set; }
        public List<byte>? ByteFieldNullableCollection { get; set; }

        public static ByteTestDto Create(
            byte byteField,
            List<byte> byteFieldCollection,
            byte? byteFieldNullable,
            List<byte>? byteFieldNullableCollection)
        {
            return new ByteTestDto
            {
                ByteField = byteField,
                ByteFieldCollection = byteFieldCollection,
                ByteFieldNullable = byteFieldNullable,
                ByteFieldNullableCollection = byteFieldNullableCollection
            };
        }
    }
}