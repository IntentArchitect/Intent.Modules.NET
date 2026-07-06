using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace SwashbuckleSettings.All.Application
{
    public class WidgetDto
    {
        public WidgetDto()
        {
            Name = null!;
        }

        public Guid WidgetId { get; set; }
        public string Name { get; set; }
        public TimeSpan ScheduledTime { get; set; }

        public static WidgetDto Create(Guid widgetId, string name, TimeSpan scheduledTime)
        {
            return new WidgetDto
            {
                WidgetId = widgetId,
                Name = name,
                ScheduledTime = scheduledTime
            };
        }
    }
}