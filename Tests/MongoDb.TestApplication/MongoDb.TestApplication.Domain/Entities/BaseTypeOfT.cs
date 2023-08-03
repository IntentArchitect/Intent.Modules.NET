using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Entities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public abstract class BaseTypeOfT<T>
    {
        [IntentManaged(Mode.Fully)]
        public BaseTypeOfT()
        {
            Id = null!;
            BaseAttribute = null!;
        }
        public string Id { get; set; }

        public T BaseAttribute { get; set; }
    }
}