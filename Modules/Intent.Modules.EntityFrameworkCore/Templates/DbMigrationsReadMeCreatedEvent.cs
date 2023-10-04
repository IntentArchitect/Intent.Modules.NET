using Intent.Modules.EntityFrameworkCore.Templates.DbMigrationsReadMe;

namespace Intent.Modules.EntityFrameworkCore.Templates;

public class DbMigrationsReadMeCreatedEvent
{
    public DbMigrationsReadMeCreatedEvent(DbMigrationsReadMeTemplate template)
    {
        Template = template;
    }
    
    public DbMigrationsReadMeTemplate Template { get; }
}