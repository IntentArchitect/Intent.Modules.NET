using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application.TypeTests
{
    public class TimeSpanTestDto
    {
        public TimeSpanTestDto()
        {
            TimeSpanFieldCollection = null!;
        }

        public TimeSpan TimeSpanField { get; set; }
        public List<TimeSpan> TimeSpanFieldCollection { get; set; }
        public TimeSpan? TimeSpanFieldNullable { get; set; }
        public List<TimeSpan>? TimeSpanFieldNullableCollection { get; set; }

        public static TimeSpanTestDto Create(
            TimeSpan timeSpanField,
            List<TimeSpan> timeSpanFieldCollection,
            TimeSpan? timeSpanFieldNullable,
            List<TimeSpan>? timeSpanFieldNullableCollection)
        {
            return new TimeSpanTestDto
            {
                TimeSpanField = timeSpanField,
                TimeSpanFieldCollection = timeSpanFieldCollection,
                TimeSpanFieldNullable = timeSpanFieldNullable,
                TimeSpanFieldNullableCollection = timeSpanFieldNullableCollection
            };
        }
    }
}