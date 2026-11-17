using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EntityFrameworkCore.Postgres.EF11.Application
{
    public record CreateBlogDto
    {
        public CreateBlogDto()
        {
            Name = null!;
            FirstPostTitle = null!;
        }

        public string Name { get; init; }
        public string FirstPostTitle { get; init; }
        public DateOnly FirstPostPublishedOn { get; init; }
    }
}