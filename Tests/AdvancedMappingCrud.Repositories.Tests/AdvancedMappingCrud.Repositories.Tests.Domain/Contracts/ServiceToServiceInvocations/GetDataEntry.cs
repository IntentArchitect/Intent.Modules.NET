using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Contracts.ServiceToServiceInvocations
{
    public record GetDataEntry
    {
        public GetDataEntry(string data)
        {
            Data = data;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected GetDataEntry()
        {
            Data = null!;
        }

        public string Data { get; init; }
    }
}