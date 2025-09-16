using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Init.Application.Defaults
{
    public class StartDefaults
    {
        public StartDefaults()
        {
            Three = null!;
        }

        public string One { get; init; } = "one";
        public int Two { get; init; }
        public string Three { get; init; }

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