using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventDto", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Eventing.Messages
{
    public class TestVODto
    {
        public TestVODto()
        {
        }

        public string Name { get; set; }

        public static TestVODto Create(string name)
        {
            return new TestVODto
            {
                Name = name
            };
        }
    }
}