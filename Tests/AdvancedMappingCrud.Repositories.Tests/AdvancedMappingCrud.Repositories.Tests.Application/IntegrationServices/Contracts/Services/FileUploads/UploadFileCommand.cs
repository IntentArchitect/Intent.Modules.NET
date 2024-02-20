using System.IO;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.IntegrationServices.Contracts.Services.FileUploads
{
    public class UploadFileCommand
    {
        public UploadFileCommand()
        {
            Content = null!;
        }

        public Stream Content { get; set; }
        public string? Filename { get; set; }
        public string? ContentType { get; set; }
        public long? ContentLength { get; set; }

        public static UploadFileCommand Create(Stream content, string? filename, string? contentType, long? contentLength)
        {
            return new UploadFileCommand
            {
                Content = content,
                Filename = filename,
                ContentType = contentType,
                ContentLength = contentLength
            };
        }
    }
}