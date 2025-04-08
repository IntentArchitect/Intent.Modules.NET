using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Parents
{
    public class UpdateParentCommandParentDetailsTagsDto
    {
        public UpdateParentCommandParentDetailsTagsDto()
        {
            TagName = null!;
            TagValue = null!;
        }

        public string TagName { get; set; }
        public string TagValue { get; set; }

        public static UpdateParentCommandParentDetailsTagsDto Create(string tagName, string tagValue)
        {
            return new UpdateParentCommandParentDetailsTagsDto
            {
                TagName = tagName,
                TagValue = tagValue
            };
        }
    }
}