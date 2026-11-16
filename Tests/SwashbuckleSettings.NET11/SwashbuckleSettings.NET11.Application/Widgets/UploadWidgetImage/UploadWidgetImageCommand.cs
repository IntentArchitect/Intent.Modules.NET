using Intent.RoslynWeaver.Attributes;
using MediatR;
using SwashbuckleSettings.NET11.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace SwashbuckleSettings.NET11.Application.Widgets.UploadWidgetImage
{
    /// <summary>
    /// Binary upload: stores the uploaded bytes in memory and returns a new id.
    /// </summary>
    public class UploadWidgetImageCommand : IRequest<Guid>, ICommand
    {
        public UploadWidgetImageCommand(Stream content)
        {
            Content = content;
        }

        public Stream Content { get; set; }
    }
}