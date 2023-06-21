using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts
{
    public record SpParameterWithName
    {
        public SpParameterWithName(string attribute)
        {
            Attribute = attribute;
        }

        public string Attribute { get; init; }
    }
}