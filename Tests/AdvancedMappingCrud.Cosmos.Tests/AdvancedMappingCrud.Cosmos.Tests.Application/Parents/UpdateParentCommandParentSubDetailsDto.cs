using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Parents
{
    public class UpdateParentCommandParentSubDetailsDto
    {
        public UpdateParentCommandParentSubDetailsDto()
        {
            SubDetailsLine1 = null!;
            SubDetailsLine2 = null!;
        }

        public string SubDetailsLine1 { get; set; }
        public string SubDetailsLine2 { get; set; }

        public static UpdateParentCommandParentSubDetailsDto Create(string subDetailsLine1, string subDetailsLine2)
        {
            return new UpdateParentCommandParentSubDetailsDto
            {
                SubDetailsLine1 = subDetailsLine1,
                SubDetailsLine2 = subDetailsLine2
            };
        }
    }
}