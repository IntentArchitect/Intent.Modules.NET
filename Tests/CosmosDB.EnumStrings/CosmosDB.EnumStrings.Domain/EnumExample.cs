using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace CosmosDB.EnumStrings.Domain
{
    public enum EnumExample
    {
        Example1 = 1,
        Example2 = 2,
        Example3 = 3
    }
}