using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace RichDomain.Domain.Entities
{
    public interface IDerivedClass : IBaseClass
    {
        string DerivedAttribute { get; }

        void DerivedOperation(string derivedAttribute, string baseClassBaseAttribute);
    }
}