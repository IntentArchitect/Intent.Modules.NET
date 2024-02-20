using System.IO;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.FileUploads.RestrictedUpload
{
    public class RestrictedUploadCommand : IRequest, ICommand
    {
        public RestrictedUploadCommand(Stream content, string? filename, string? contentType, long? contentLength)
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