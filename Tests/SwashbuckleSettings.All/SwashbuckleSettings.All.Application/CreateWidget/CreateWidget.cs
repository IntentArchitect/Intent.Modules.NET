using Intent.RoslynWeaver.Attributes;
using MediatR;
using SwashbuckleSettings.All.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace SwashbuckleSettings.All.Application.CreateWidget
{
    public class CreateWidget : IRequest, ICommand
    {
        public CreateWidget(WidgetDto widget)
        {
            Widget = widget;
        }

        public WidgetDto Widget { get; set; }
    }
}