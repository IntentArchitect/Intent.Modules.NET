using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace SwashbuckleSettings.NET11.Application.Widgets.UploadWidgetImage
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UploadWidgetImageCommandHandler : IRequestHandler<UploadWidgetImageCommand, Guid>
    {
        [IntentManaged(Mode.Merge)]
        public UploadWidgetImageCommandHandler()
        {
        }

        /// <summary>
        /// Binary upload: stores the uploaded bytes in memory and returns a new id.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> Handle(UploadWidgetImageCommand request, CancellationToken cancellationToken)
        {
            using var buffer = new MemoryStream();
            await request.Content.CopyToAsync(buffer, cancellationToken);
            var id = Guid.NewGuid();
            InMemoryWidgetStore.Images[id] = buffer.ToArray();
            return id;
        }
    }
}
