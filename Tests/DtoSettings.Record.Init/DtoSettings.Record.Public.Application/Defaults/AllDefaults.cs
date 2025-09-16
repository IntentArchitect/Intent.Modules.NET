using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Record.Public.Application.Defaults
{
    public record AllDefaults
    {
        public AllDefaults()
        {
        }

        public string One { get; init; } = "one";
        public int Two { get; init; } = 2;
        public string Three { get; init; } = "three";

        public static AllDefaults Create(string one = "one", int two = 2, string three = "three")
        {
            return new AllDefaults
            {
                One = one,
                Two = two,
                Three = three
            };
        }
    }
}