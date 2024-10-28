using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace FastEndpointsTest.Domain.Entities.CRUD
{
    public class CompositeOfAggrLong
    {
        public long Id { get; set; }

        public string Attribute { get; set; }
    }
}