using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application.TypeTests
{
    public class DateTimeTestDto
    {
        public DateTimeTestDto()
        {
            DateTimeFieldCollection = null!;
        }

        public DateTime DateTimeField { get; set; }
        public List<DateTime> DateTimeFieldCollection { get; set; }
        public DateTime? DateTimeFieldNullable { get; set; }
        public List<DateTime>? DateTimeFieldNullableCollection { get; set; }

        public static DateTimeTestDto Create(
            DateTime dateTimeField,
            List<DateTime> dateTimeFieldCollection,
            DateTime? dateTimeFieldNullable,
            List<DateTime>? dateTimeFieldNullableCollection)
        {
            return new DateTimeTestDto
            {
                DateTimeField = dateTimeField,
                DateTimeFieldCollection = dateTimeFieldCollection,
                DateTimeFieldNullable = dateTimeFieldNullable,
                DateTimeFieldNullableCollection = dateTimeFieldNullableCollection
            };
        }
    }
}