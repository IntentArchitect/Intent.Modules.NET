using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Record.Public.Application.Defaults
{
    public record StartDefaults
    {
        public StartDefaults()
        {
            Three = null!;
        }

        public string One { get; set; } = "one";
        public int Two { get; set; }
        public string Three { get; set; }

        public static StartDefaults Create(string one, int two, string three)
        {
            return new StartDefaults
            {
                One = one,
                Two = two,
                Three = three
            };
        }
    }
}