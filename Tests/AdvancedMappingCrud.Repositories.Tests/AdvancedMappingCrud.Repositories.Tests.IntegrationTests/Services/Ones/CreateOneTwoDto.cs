using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Ones
{
    public class CreateOneTwoDto
    {
        public CreateOneTwoDto()
        {
            TwoName = null!;
        }

        public string TwoName { get; set; }

        public static CreateOneTwoDto Create(string twoName)
        {
            return new CreateOneTwoDto
            {
                TwoName = twoName
            };
        }
    }
}