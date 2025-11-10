using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts
{
    public record UdttDataEntityModel
    {
        public UdttDataEntityModel(long dataEntityID)
        {
            DataEntityID = dataEntityID;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected UdttDataEntityModel()
        {
        }

        public long DataEntityID { get; init; }
    }
}