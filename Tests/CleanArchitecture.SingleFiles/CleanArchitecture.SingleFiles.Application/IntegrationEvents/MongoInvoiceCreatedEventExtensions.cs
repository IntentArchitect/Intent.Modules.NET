using CleanArchitecture.SingleFiles.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.DomainMapping.MessageExtensions", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Messages
{
    public static class MongoInvoiceCreatedEventExtensions
    {
        public static MongoInvoiceCreatedEvent MapToMongoInvoiceCreatedEvent(this MongoInvoice projectFrom)
        {
            return new MongoInvoiceCreatedEvent
            {
                Description = projectFrom.Description,
            };
        }
    }
}