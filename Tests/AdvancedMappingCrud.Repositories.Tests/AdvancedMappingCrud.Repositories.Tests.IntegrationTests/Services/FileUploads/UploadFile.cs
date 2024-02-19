using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.FileUploads
{
    public class UploadFile
    {
        public UploadFile()
        {
            Content = null!;
        }

        public Stream Content { get; set; }
        public string? Filename { get; set; }
        public string? ContentType { get; set; }

        public static UploadFile Create(Stream content, string? filename, string? contentType)
        {
            return new UploadFile
            {
                Content = content,
                Filename = filename,
                ContentType = contentType
            };
        }
    }
}