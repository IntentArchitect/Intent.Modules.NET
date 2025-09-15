using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Enums
{
    public enum BuildingType
    {
        NA = 1,
        House = 1,
        Flat = 2,
        Office = 3
    }
}