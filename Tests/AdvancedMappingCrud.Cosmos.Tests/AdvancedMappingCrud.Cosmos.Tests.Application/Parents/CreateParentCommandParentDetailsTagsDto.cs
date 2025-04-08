using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Parents
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