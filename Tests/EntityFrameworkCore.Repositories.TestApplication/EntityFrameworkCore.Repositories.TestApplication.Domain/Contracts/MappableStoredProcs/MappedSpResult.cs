using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts.MappableStoredProcs
{
    public record MappedSpResult
    {
        public MappedSpResult(MappedSpResultItem result, string simpleString)
        {
            Result = result;
            SimpleString = simpleString;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected MappedSpResult()
        {
            Result = null!;
            SimpleString = null!;
        }

        public MappedSpResultItem Result { get; init; }
        public string SimpleString { get; init; }
    }
}