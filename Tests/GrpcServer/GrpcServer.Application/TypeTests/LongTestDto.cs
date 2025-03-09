using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application.TypeTests
{
    public class LongTestDto
    {
        public LongTestDto()
        {
            LongFieldCollection = null!;
        }

        public long LongField { get; set; }
        public List<long> LongFieldCollection { get; set; }
        public long? LongFieldNullable { get; set; }
        public List<long>? LongFieldNullableCollection { get; set; }

        public static LongTestDto Create(
            long longField,
            List<long> longFieldCollection,
            long? longFieldNullable,
            List<long>? longFieldNullableCollection)
        {
            return new LongTestDto
            {
                LongField = longField,
                LongFieldCollection = longFieldCollection,
                LongFieldNullable = longFieldNullable,
                LongFieldNullableCollection = longFieldNullableCollection
            };
        }
    }
}