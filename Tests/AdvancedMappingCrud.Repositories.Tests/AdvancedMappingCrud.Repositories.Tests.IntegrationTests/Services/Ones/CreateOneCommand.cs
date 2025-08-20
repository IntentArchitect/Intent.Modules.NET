using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Ones
{
    public class CreateOneCommand
    {
        public CreateOneCommand()
        {
            OneName = null!;
            Two = null!;
        }

        public string OneName { get; set; }
        public CreateOneTwoDto Two { get; set; }

        public static CreateOneCommand Create(string oneName, CreateOneTwoDto two)
        {
            return new CreateOneCommand
            {
                OneName = oneName,
                Two = two
            };
        }
    }
}