using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Domain.Enums
{
    public enum EnumWithoutValues
    {
        LiteralOne,
        LiteralTwo,
        LiteralThree
    }
}