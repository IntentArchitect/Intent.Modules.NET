using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.IntegrationServices.TestVersionedProxy
{
    public class TestCommandV1
    {
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