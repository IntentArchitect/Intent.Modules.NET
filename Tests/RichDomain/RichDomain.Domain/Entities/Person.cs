using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace RichDomain.Domain.Entities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public partial class Person : IPerson
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public Person(string firstName)
        {
            FirstName = firstName;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public void UpdatePerson(string firstName)
        {
            FirstName = firstName;
        }
    }
}