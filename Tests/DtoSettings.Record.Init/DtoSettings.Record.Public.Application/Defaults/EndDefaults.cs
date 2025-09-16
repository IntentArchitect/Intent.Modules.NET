using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Record.Public.Application.Defaults
{
    public record EndDefaults
    {
        public EndDefaults()
        {
            One = null!;
        }

        public string One { get; init; }
        public int Two { get; init; }
        public string Three { get; init; } = "three";

        public static EndDefaults Create(string one, int two, string three = "three")
        {
            return new EndDefaults
            {
                One = one,
                Two = two,
                Three = three
            };
        }
    }
}