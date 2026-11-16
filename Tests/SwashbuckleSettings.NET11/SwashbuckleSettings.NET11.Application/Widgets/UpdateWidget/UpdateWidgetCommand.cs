using Intent.RoslynWeaver.Attributes;
using MediatR;
using SwashbuckleSettings.NET11.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace SwashbuckleSettings.NET11.Application.Widgets.UpdateWidget
{
    /// <summary>
    /// The route carries {id} and the command body also has Id; the OpenAPI body schema must not show id.
    /// </summary>
    public class UpdateWidgetCommand : IRequest, ICommand
    {
        public UpdateWidgetCommand(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}