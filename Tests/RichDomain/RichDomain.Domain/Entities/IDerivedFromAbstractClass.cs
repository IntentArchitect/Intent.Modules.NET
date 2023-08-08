using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace RichDomain.Domain.Entities
{
    public interface IDerivedFromAbstractClass : IAbstractBaseClass
    {
        string DerivedAttribute { get; }

        void DerivedOperation(string derivedAttribute, string abstractBaseClassAbstractBaseAttribute);
    }
}