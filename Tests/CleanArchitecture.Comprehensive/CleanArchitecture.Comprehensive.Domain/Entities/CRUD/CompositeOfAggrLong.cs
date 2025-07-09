using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.CRUD
{
    public class CompositeOfAggrLong
    {
        public CompositeOfAggrLong()
        {
            Attribute = null!;
        }

        public long Id { get; set; }

        public string Attribute { get; set; }
    }
}