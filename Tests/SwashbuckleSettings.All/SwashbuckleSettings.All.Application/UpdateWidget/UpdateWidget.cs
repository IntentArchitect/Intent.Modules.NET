using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SwashbuckleSettings.All.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace SwashbuckleSettings.All.Application.UpdateWidget
{
    public class UpdateWidget : IRequest, ICommand
    {
        public UpdateWidget(WidgetDto widget, Guid widgetId)
        {
            Widget = widget;
            WidgetId = widgetId;
        }

        public WidgetDto Widget { get; set; }
        public Guid WidgetId { get; set; }
    }
}