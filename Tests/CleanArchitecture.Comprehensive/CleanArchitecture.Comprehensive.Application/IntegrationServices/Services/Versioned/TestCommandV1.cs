using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.IntegrationServices.Services.Versioned
{
    public class TestCommandV1
    {
        public TestCommandV1()
        {
            Value = null!;
        }
        public string Value { get; set; }

        public static TestCommandV1 Create(string value)
        {
            return new TestCommandV1
            {
                Value = value
            };
        }
    }
}