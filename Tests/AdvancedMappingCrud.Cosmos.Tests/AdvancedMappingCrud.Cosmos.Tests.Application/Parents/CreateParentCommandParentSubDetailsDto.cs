using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Parents
{
    public class CreateParentCommandParentSubDetailsDto
    {
        public CreateParentCommandParentSubDetailsDto()
        {
            SubDetailsLine1 = null!;
            SubDetailsLine2 = null!;
        }

        public string SubDetailsLine1 { get; set; }
        public string SubDetailsLine2 { get; set; }

        public static CreateParentCommandParentSubDetailsDto Create(string subDetailsLine1, string subDetailsLine2)
        {
            return new CreateParentCommandParentSubDetailsDto
            {
                SubDetailsLine1 = subDetailsLine1,
                SubDetailsLine2 = subDetailsLine2
            };
        }
    }
}