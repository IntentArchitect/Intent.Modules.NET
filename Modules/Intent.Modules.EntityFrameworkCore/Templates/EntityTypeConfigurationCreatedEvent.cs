using Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;

namespace Intent.Modules.EntityFrameworkCore.Templates;

public class EntityTypeConfigurationCreatedEvent
{
    public EntityTypeConfigurationCreatedEvent(EntityTypeConfigurationTemplate template)
    {
        Template = template;
    }

    public EntityTypeConfigurationTemplate Template { get; set; }
}