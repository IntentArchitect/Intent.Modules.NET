using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Internal.Application.Defaults
{
    public class EndDefaults
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

        public string One { get; internal set; }
        public int Two { get; internal set; }
        public string Three { get; internal set; } = "three";
    }
}