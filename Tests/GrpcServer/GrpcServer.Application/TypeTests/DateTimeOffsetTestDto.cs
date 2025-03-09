using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application.TypeTests
{
    public class DateTimeOffsetTestDto
    {
        public DateTimeOffsetTestDto()
        {
            DateTimeOffsetFieldCollection = null!;
        }

        public DateTimeOffset DateTimeOffsetField { get; set; }
        public List<DateTimeOffset> DateTimeOffsetFieldCollection { get; set; }
        public DateTimeOffset? DateTimeOffsetFieldNullable { get; set; }
        public List<DateTimeOffset>? DateTimeOffsetFieldNullableCollection { get; set; }

        public static DateTimeOffsetTestDto Create(
            DateTimeOffset dateTimeOffsetField,
            List<DateTimeOffset> dateTimeOffsetFieldCollection,
            DateTimeOffset? dateTimeOffsetFieldNullable,
            List<DateTimeOffset>? dateTimeOffsetFieldNullableCollection)
        {
            return new DateTimeOffsetTestDto
            {
                DateTimeOffsetField = dateTimeOffsetField,
                DateTimeOffsetFieldCollection = dateTimeOffsetFieldCollection,
                DateTimeOffsetFieldNullable = dateTimeOffsetFieldNullable,
                DateTimeOffsetFieldNullableCollection = dateTimeOffsetFieldNullableCollection
            };
        }
    }
}