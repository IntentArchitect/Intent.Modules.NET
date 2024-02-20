using System.IO;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Common
{
    public class SimpleFileDownloadDto
    {
        public SimpleFileDownloadDto()
        {
            Content = null!;
        }

        public Stream Content { get; set; }

        public static SimpleFileDownloadDto Create(Stream content)
        {
            return new SimpleFileDownloadDto
            {
                Content = content
            };
        }
    }
}