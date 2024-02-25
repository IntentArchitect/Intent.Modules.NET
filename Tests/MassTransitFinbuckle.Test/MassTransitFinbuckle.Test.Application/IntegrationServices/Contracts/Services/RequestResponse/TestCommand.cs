using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.ClientContracts.DtoContract", Version = "2.0")]

namespace MassTransitFinbuckle.Test.Application.IntegrationServices.Contracts.Services.RequestResponse
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