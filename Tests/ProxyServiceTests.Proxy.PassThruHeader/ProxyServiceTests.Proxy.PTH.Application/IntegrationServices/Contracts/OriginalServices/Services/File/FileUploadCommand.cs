using System.IO;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace ProxyServiceTests.Proxy.PTH.Application.IntegrationServices.Contracts.OriginalServices.Services.File
{
    public class FileUploadCommand
    {
        public FileUploadCommand()
        {
            Content = null!;
        }

        public Stream Content { get; set; }
        public string? Filename { get; set; }
        public string? ContentType { get; set; }
        public long? ContentLength { get; set; }

        public static FileUploadCommand Create(Stream content, string? filename, string? contentType, long? contentLength)
        {
            return new FileUploadCommand
            {
                Content = content,
                Filename = filename,
                ContentType = contentType,
                ContentLength = contentLength
            };
        }
    }
}