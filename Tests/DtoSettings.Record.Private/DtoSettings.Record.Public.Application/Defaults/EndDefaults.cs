using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Record.Public.Application.Defaults
{
    public record EndDefaults
    {
        public EndDefaults(string one, int two, string three = "three")
        {
            One = one;
            Two = two;
            Three = three;
        }

        protected EndDefaults()
        {
            One = null!;
        }

        public string One { get; private set; }
        public int Two { get; private set; }
        public string Three { get; private set; } = "three";

        public static EndDefaults Create(string one, int two, string three = "three")
        {
            return new EndDefaults(one, two, three);
        }
    }
}