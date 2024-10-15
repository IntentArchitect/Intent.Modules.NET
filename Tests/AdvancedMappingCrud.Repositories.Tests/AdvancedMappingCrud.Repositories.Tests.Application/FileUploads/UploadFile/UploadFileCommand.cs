using System;
using System.IO;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.FileUploads.UploadFile
{
    public class UploadFileCommand : IRequest<Guid>, ICommand
    {
        public UploadFileCommand(Stream content,
            string? filename = null,
            string? contentType = null,
            long? contentLength = null)
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