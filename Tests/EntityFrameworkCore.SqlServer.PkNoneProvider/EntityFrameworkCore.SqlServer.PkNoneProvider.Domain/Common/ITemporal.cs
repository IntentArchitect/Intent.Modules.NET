using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.TemporalTables.TemporalInterface", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Common
{
    // Marker interface for temporal domain entities
    public interface ITemporal
    {
    }
}