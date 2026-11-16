using Intent.RoslynWeaver.Attributes;
using MediatR;
using SwashbuckleSettings.NET11.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace SwashbuckleSettings.NET11.Application.Widgets.CreateWidget
{
    public class CreateWidgetCommand : IRequest<Guid>, ICommand
    {
        public CreateWidgetCommand(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}