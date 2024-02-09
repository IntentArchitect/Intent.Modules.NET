using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Redis.Om.Repositories.Templates.RedisOmDocumentInterface", Version = "1.0")]

namespace Redis.Om.Repositories.Domain.Repositories.Documents
{
    public interface ILineItemDocument
    {
        string Id { get; }
        string Description { get; }
        int Quantity { get; }
        IReadOnlyList<string> Tags { get; }
        IProductDocument Product { get; }
    }
}