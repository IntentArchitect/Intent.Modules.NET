using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts
{
    public record SpResult
    {
        public SpResult(string data)
        {
            Data = data;
        }

        public string Data { get; init; }
    }
}