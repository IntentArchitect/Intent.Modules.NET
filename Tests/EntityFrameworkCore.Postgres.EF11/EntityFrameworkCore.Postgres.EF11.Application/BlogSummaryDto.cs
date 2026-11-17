using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.EF11.Application
{
    public record BlogSummaryDto
    {
        public BlogSummaryDto()
        {
            Name = null!;
        }

        public Guid Id { get; init; }
        public string Name { get; init; }
        public int PostCount { get; init; }
    }
}