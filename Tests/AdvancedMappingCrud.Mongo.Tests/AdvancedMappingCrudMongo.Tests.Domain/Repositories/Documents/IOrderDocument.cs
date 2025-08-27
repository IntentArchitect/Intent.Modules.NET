using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentInterface", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Domain.Repositories.Documents
{
    public interface IOrderDocument
    {
        string Id { get; }
        string CustomerId { get; }
        string RefNo { get; }
        DateTime OrderDate { get; }
        string ExternalRef { get; }
        IEnumerable<IOrderItemDocument> OrderItems { get; }
    }
}