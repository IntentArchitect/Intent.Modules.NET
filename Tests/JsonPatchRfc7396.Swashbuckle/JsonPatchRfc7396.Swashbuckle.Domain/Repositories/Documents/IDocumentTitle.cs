using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbValueObjectDocumentInterface", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Domain.Repositories.Documents
{
    public interface IDocumentTitle
    {
        bool ExampleMethod(string exampleParam);
    }
}