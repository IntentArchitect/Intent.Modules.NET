using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Domain.Contracts.DDD
{
    public record DataContractObject
    {
        public DataContractObject()
        {
        }
    }
}