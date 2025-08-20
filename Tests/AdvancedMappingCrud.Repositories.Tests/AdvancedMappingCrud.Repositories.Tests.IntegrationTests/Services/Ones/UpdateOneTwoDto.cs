using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Ones
{
    public class UpdateOneTwoDto
    {
        public UpdateOneTwoDto()
        {
            TwoName = null!;
        }
        public string TwoName { get; set; }

        public static UpdateOneTwoDto Create(string twoName)
        {
            return new UpdateOneTwoDto
            {
                TwoName = twoName
            };
        }
    }
}