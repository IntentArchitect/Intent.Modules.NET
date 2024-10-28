using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace FastEndpointsTest.Domain.Contracts.DDD
{
    public record DataContractObject
    {
        public DataContractObject()
        {
        }
    }
}