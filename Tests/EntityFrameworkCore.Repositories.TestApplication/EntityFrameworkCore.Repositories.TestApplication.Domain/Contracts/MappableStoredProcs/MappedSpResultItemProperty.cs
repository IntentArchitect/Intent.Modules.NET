using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts.MappableStoredProcs
{
    public record MappedSpResultItemProperty
    {
        public MappedSpResultItemProperty(string prop1)
        {
            Prop1 = prop1;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected MappedSpResultItemProperty()
        {
            Prop1 = null!;
        }

        public string Prop1 { get; init; }
    }
}