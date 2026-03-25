using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbValueObjectDocument", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Infrastructure.Persistence.Documents
{
    public class DocumentContent
    {
        private readonly string _exampleParam;

        public DocumentContent(string exampleParam)
        {
            _exampleParam = exampleParam;
        }
    }
}