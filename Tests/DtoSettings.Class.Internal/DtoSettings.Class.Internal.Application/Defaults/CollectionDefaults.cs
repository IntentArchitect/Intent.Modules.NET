using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Internal.Application.Defaults
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

        public string One { get; internal set; }
        public List<string> Two { get; internal set; }
        public List<string> Three { get; internal set; } = [];
    }
}