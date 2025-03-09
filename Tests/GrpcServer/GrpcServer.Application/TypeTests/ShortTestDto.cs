using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application.TypeTests
{
    public class ShortTestDto
    {
        public ShortTestDto()
        {
            ShortFieldCollection = null!;
        }

        public short ShortField { get; set; }
        public List<short> ShortFieldCollection { get; set; }
        public short? ShortFieldNullable { get; set; }
        public List<short>? ShortFieldNullableCollection { get; set; }

        public static ShortTestDto Create(
            short shortField,
            List<short> shortFieldCollection,
            short? shortFieldNullable,
            List<short>? shortFieldNullableCollection)
        {
            return new ShortTestDto
            {
                ShortField = shortField,
                ShortFieldCollection = shortFieldCollection,
                ShortFieldNullable = shortFieldNullable,
                ShortFieldNullableCollection = shortFieldNullableCollection
            };
        }
    }
}