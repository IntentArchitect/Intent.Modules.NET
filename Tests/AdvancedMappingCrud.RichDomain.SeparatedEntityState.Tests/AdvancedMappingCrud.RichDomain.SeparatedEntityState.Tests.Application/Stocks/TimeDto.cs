using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Stocks
{
    public class TimeDto
    {
        public TimeDto()
        {
        }

        public DateTime CurrentDateTime { get; set; }

        public static TimeDto Create(DateTime currentDateTime)
        {
            return new TimeDto
            {
                CurrentDateTime = currentDateTime
            };
        }
    }
}