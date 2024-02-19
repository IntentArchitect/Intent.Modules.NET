using System;
using System.IO;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.FileUploads.UploadFile
{
    public class UploadFile : IRequest<Guid>, ICommand
    {
        public UploadFile(Stream content, string? filename, string? contentType)
        {
            Content = content;
            Filename = filename;
            ContentType = contentType;
        }

        public Stream Content { get; set; }
        public string? Filename { get; set; }
        public string? ContentType { get; set; }
    }
}