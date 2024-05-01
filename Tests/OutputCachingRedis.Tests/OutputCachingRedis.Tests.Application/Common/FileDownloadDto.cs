using System.IO;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace OutputCachingRedis.Tests.Application.Common
{
    public class FileDownloadDto
    {
        public FileDownloadDto()
        {
            Content = null!;
        }

        public Stream Content { get; set; }
        public string? Filename { get; set; }
        public string? ContentType { get; set; }

        public static FileDownloadDto Create(Stream content, string? filename, string? contentType)
        {
            return new FileDownloadDto
            {
                Content = content,
                Filename = filename,
                ContentType = contentType
            };
        }
    }
}