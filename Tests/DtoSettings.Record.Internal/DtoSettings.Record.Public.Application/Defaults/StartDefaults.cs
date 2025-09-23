using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Record.Public.Application.Defaults
{
    public record StartDefaults
    {
        public StartDefaults(string one, int two, string three)
        {
            One = one;
            Two = two;
            Three = three;
        }

        protected StartDefaults()
        {
            Three = null!;
        }

        public string One { get; internal set; } = "one";
        public int Two { get; internal set; }
        public string Three { get; internal set; }
    }
}