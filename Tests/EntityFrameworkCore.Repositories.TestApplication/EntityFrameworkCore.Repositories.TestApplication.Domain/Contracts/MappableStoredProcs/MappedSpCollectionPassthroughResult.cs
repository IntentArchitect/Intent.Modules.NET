using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts.MappableStoredProcs
{
    public record MappedSpCollectionPassthroughResult
    {
        public MappedSpCollectionPassthroughResult(string attribute)
        {
            Attribute = attribute;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected MappedSpCollectionPassthroughResult()
        {
            Attribute = null!;
        }

        public string Attribute { get; init; }
    }
}