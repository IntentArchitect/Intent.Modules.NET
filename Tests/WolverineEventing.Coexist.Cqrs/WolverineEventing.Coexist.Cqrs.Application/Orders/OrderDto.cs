using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace WolverineEventing.Coexist.Cqrs.Application.Orders
{
    public record OrderDto
    {
        public OrderDto()
        {
            Field = null!;
        }

        public string Field { get; init; }
    }
}