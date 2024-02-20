using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.FileUploads
{
    public class SimpleUploadCommand
    {
        public SimpleUploadCommand()
        {
            Content = null!;
        }

        public Stream Content { get; set; }

        public static SimpleUploadCommand Create(Stream content)
        {
            return new SimpleUploadCommand
            {
                Content = content
            };
        }
    }
}