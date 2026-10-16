using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBValueObjectDocumentInterface", Version = "1.0")]

namespace CosmosDB.Domain.Repositories.Documents
{
    public interface IScheduleDocument
    {
        string Name { get; }
        IReadOnlyList<IScheduleSlotDocument> Slots { get; }
        IScheduleSlotDocument? PrimarySlot { get; }
    }
}