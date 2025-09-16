using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Private.Application.Defaults
{
    public class StartDefaults
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

        public string One { get; private set; } = "one";
        public int Two { get; private set; }
        public string Three { get; private set; }

        public static StartDefaults Create(string one, int two, string three)
        {
            return new StartDefaults(one, two, three);
        }
    }
}