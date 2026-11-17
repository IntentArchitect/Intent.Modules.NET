using Intent.RoslynWeaver.Attributes;
using MediatR;
using SwashbuckleSettings.NET11.Application.Common.Interfaces;
using SwashbuckleSettings.NET11.Application.Common.Patching;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace SwashbuckleSettings.NET11.Application.Widgets.PatchWidget
{
    /// <summary>
    /// JSON merge patch (RFC 7396) of a widget's name.
    /// </summary>
    public class PatchWidgetCommand : IRequest<WidgetDto>, ICommand, IBypassPipelineValidation
    {
        public PatchWidgetCommand(Guid id, IPatchExecutor<PatchWidgetCommand> patchExecutor)
        {
            Id = id;
            PatchExecutor = patchExecutor;
        }

        public Guid Id { get; private set; }
        public string? Name { get; set; }
        public IPatchExecutor<PatchWidgetCommand> PatchExecutor { get; private set; }
    }
}