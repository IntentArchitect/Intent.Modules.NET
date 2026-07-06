using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SwashbuckleSettings.All.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace SwashbuckleSettings.All.Application.ReplaceWidget
{
    public class ReplaceWidget : IRequest, ICommand
    {
        public ReplaceWidget(Guid widgetId, string name, TimeSpan scheduledTime)
        {
            WidgetId = widgetId;
            Name = name;
            ScheduledTime = scheduledTime;
        }

        public Guid WidgetId { get; set; }
        public string Name { get; set; }
        public TimeSpan ScheduledTime { get; set; }
    }
}