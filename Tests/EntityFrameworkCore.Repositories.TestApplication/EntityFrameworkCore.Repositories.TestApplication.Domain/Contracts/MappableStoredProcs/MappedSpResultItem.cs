using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts.MappableStoredProcs
{
    public record MappedSpResultItem
    {
        public MappedSpResultItem(string simpleProp, MappedSpResultItemProperty complexProp)
        {
            SimpleProp = simpleProp;
            ComplexProp = complexProp;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected MappedSpResultItem()
        {
            SimpleProp = null!;
            ComplexProp = null!;
        }

        public string SimpleProp { get; init; }
        public MappedSpResultItemProperty ComplexProp { get; init; }
    }
}