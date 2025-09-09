using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories.Documents.Indexes
{
    public interface ICompoundIndexEntitySingleChildDocument
    {
        string CompoundOne { get; }
        string CompoundTwo { get; }
    }
}