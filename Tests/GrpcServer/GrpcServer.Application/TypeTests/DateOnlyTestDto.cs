using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application.TypeTests
{
    public class DateOnlyTestDto
    {
        public DateOnlyTestDto()
        {
            DateOnlyFieldCollection = null!;
        }

        public DateOnly DateOnlyField { get; set; }
        public List<DateOnly> DateOnlyFieldCollection { get; set; }
        public DateOnly? DateOnlyFieldNullable { get; set; }
        public List<DateOnly>? DateOnlyFieldNullableCollection { get; set; }

        public static DateOnlyTestDto Create(
            DateOnly dateOnlyField,
            List<DateOnly> dateOnlyFieldCollection,
            DateOnly? dateOnlyFieldNullable,
            List<DateOnly>? dateOnlyFieldNullableCollection)
        {
            return new DateOnlyTestDto
            {
                DateOnlyField = dateOnlyField,
                DateOnlyFieldCollection = dateOnlyFieldCollection,
                DateOnlyFieldNullable = dateOnlyFieldNullable,
                DateOnlyFieldNullableCollection = dateOnlyFieldNullableCollection
            };
        }
    }
}