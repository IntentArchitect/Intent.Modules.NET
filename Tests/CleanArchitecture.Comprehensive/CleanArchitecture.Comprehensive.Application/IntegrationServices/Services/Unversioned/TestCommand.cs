using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.IntegrationServices.Services.Unversioned
{
    public class TestCommand
    {
        public TestCommand()
        {
            Value = null!;
        }
        public string Value { get; set; }

        public static TestCommand Create(string value)
        {
            return new TestCommand
            {
                Value = value
            };
        }
    }
}