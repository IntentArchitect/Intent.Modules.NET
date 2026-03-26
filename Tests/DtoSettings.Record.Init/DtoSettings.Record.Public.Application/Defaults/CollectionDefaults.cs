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

        public string One { get; init; }
        public List<string> Two { get; init; }
        public List<string> Three { get; init; } = [];

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