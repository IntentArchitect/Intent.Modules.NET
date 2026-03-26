using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Private.Application.Defaults
{
    public class CollectionDefaults
    {
        public CollectionDefaults(string one, List<string> two, List<string>? three = null)
        {
            One = one;
            Two = two;
            Three = three ?? [];
        }

        protected CollectionDefaults()
        {
            One = null!;
            Two = null!;
        }

        public string One { get; private set; }
        public List<string> Two { get; private set; }
        public List<string> Three { get; private set; } = [];

        public static CollectionDefaults Create(string one, List<string> two, List<string>? three = null)
        {
            return new CollectionDefaults(one, two, three);
        }
    }
}