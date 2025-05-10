using System.IO;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.Proxy.PTH.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace ProxyServiceTests.Proxy.PTH.Application.File.FileUpload
{
    public class FileUploadCommand : IRequest, ICommand
    {
        public FileUploadCommand(Stream content, string? filename, string? contentType, long? contentLength)
        {
            Content = content;
            Filename = filename;
            ContentType = contentType;
            ContentLength = contentLength;
        }

        public Stream Content { get; set; }
        public string? Filename { get; set; }
        public string? ContentType { get; set; }
        public long? ContentLength { get; set; }
    }
}