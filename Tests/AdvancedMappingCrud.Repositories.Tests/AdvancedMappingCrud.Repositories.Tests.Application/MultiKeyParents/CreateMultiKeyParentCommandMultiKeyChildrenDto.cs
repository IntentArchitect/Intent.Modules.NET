using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.MultiKeyParents
{
    public class CreateMultiKeyParentCommandMultiKeyChildrenDto
    {
        public CreateMultiKeyParentCommandMultiKeyChildrenDto()
        {
            ChildName = null!;
        }

        public string ChildName { get; set; }

        public static CreateMultiKeyParentCommandMultiKeyChildrenDto Create(string childName)
        {
            return new CreateMultiKeyParentCommandMultiKeyChildrenDto
            {
                ChildName = childName
            };
        }
    }
}