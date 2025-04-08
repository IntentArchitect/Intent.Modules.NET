using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Parents
{
    public class CreateParentCommandParentDetailsTagsDto
    {
        public CreateParentCommandParentDetailsTagsDto()
        {
            TagName = null!;
            TagValue = null!;
        }

        public string TagName { get; set; }
        public string TagValue { get; set; }

        public static CreateParentCommandParentDetailsTagsDto Create(string tagName, string tagValue)
        {
            return new CreateParentCommandParentDetailsTagsDto
            {
                TagName = tagName,
                TagValue = tagValue
            };
        }
    }
}