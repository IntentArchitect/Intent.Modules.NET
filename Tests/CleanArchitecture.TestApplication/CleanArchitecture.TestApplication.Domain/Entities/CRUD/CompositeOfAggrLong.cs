using Intent.RoslynWeaver.Attributes;

namespace CleanArchitecture.TestApplication.Domain.Entities.CRUD
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
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