using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.OperationMapping;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence.Configurations.OperationMapping
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserName)
                .IsRequired();

            builder.OwnsMany(x => x.TaskLists, ConfigureTaskLists);

            builder.Ignore(e => e.DomainEvents);
        }

        public static void ConfigureTaskLists(OwnedNavigationBuilder<User, TaskList> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.UserId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.OwnsMany(x => x.TaskItems, ConfigureTaskItems);
        }

        public static void ConfigureTaskItems(OwnedNavigationBuilder<TaskList, TaskItem> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.TaskListId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.TaskListId)
                .IsRequired();

            builder.OwnsMany(x => x.SubTasks, ConfigureSubTasks);
        }

        public static void ConfigureSubTasks(OwnedNavigationBuilder<TaskItem, SubTask> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.TaskItemId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.TaskItemId)
                .IsRequired();
        }
    }
}