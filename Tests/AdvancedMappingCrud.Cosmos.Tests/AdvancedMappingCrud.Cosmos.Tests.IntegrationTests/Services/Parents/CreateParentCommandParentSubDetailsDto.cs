using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Parents
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