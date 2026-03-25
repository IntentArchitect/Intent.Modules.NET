using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbValueObjectDocumentInterface", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Domain.Repositories.Documents
{
    public interface IDocumentContent
    {
        bool ExampleMethod(string exampleParam);
    }
}