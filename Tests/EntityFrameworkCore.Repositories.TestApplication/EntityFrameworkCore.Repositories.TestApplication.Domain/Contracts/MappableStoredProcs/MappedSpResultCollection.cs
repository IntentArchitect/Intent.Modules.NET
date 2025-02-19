using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts.MappableStoredProcs
{
    public record MappedSpResultCollection
    {
        public MappedSpResultCollection(IEnumerable<MappedSpResultItem> result, string simpleString)
        {
            Result = result;
            SimpleString = simpleString;
        }

        public IEnumerable<MappedSpResultItem> Result { get; init; }
        public string SimpleString { get; init; }
    }
}