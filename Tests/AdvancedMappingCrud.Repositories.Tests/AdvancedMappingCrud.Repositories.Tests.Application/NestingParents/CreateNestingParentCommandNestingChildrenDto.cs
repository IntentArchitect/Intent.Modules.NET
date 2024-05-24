using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.NestingParents
{
    public class CreateNestingParentCommandNestingChildrenDto
    {
        public CreateNestingParentCommandNestingChildrenDto()
        {
            Description = null!;
            ChildChild = null!;
        }

        public string Description { get; set; }
        public ManualChildChildDto ChildChild { get; set; }

        public static CreateNestingParentCommandNestingChildrenDto Create(
            string description,
            ManualChildChildDto childChild)
        {
            return new CreateNestingParentCommandNestingChildrenDto
            {
                Description = description,
                ChildChild = childChild
            };
        }
    }
}