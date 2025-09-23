using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Protected.Application.Defaults
{
    public class AllDefaults
    {
        public AllDefaults(string one = "one", int two = 2, string three = "three")
        {
            One = one;
            Two = two;
            Three = three;
        }

        protected AllDefaults()
        {
        }

        public string One { get; protected set; } = "one";
        public int Two { get; protected set; } = 2;
        public string Three { get; protected set; } = "three";

        public static AllDefaults Create(string one = "one", int two = 2, string three = "three")
        {
            return new AllDefaults(one, two, three);
        }
    }
}