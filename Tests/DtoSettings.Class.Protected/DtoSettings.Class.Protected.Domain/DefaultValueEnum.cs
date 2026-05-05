using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace DtoSettings.Class.Protected.Domain
{
    public enum DefaultValueEnum
    {
        One = 1,
        Two = 2,
        Three = 3
    }
}