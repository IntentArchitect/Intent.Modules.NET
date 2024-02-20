using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Common
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