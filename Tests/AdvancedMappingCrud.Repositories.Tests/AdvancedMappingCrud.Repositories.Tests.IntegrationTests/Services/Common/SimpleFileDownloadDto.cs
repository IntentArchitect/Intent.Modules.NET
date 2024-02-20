using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Common
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