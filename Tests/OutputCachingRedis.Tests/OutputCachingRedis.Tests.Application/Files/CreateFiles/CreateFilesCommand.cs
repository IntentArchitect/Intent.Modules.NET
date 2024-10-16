using System;
using System.IO;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using OutputCachingRedis.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace OutputCachingRedis.Tests.Application.Files.CreateFiles
{
    public class CreateFilesCommand : IRequest<Guid>, ICommand
    {
        public CreateFilesCommand(Stream content, string? filename, string? contentType, long? contentLength)
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