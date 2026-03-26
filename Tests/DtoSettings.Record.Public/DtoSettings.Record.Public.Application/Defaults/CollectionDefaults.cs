using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Record.Public.Application.Defaults
{
    public record CollectionDefaults
    {
        public CollectionDefaults()
        {
            One = null!;
            Two = null!;
        }

        public string One { get; set; }
        public List<string> Two { get; set; }
        public List<string> Three { get; set; } = [];

        public static CollectionDefaults Create(string one, List<string> two, List<string>? three = null)
        {
            return new CollectionDefaults
            {
                One = one,
                Two = two,
                Three = three ?? []
            };
        }
    }
}