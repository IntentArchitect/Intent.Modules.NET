using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application.TypeTests
{
    public class DecimalTestDto
    {
        public DecimalTestDto()
        {
            DecimalFieldCollection = null!;
        }

        public decimal DecimalField { get; set; }
        public List<decimal> DecimalFieldCollection { get; set; }
        public decimal? DecimalFieldNullable { get; set; }
        public List<decimal>? DecimalFieldNullableCollection { get; set; }

        public static DecimalTestDto Create(
            decimal decimalField,
            List<decimal> decimalFieldCollection,
            decimal? decimalFieldNullable,
            List<decimal>? decimalFieldNullableCollection)
        {
            return new DecimalTestDto
            {
                DecimalField = decimalField,
                DecimalFieldCollection = decimalFieldCollection,
                DecimalFieldNullable = decimalFieldNullable,
                DecimalFieldNullableCollection = decimalFieldNullableCollection
            };
        }
    }
}