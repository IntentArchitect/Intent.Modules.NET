using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Parents
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