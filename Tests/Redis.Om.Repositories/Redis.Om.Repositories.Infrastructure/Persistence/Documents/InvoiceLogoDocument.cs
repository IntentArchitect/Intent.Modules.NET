using System;
using Intent.RoslynWeaver.Attributes;
using Redis.Om.Repositories.Domain.Entities;
using Redis.Om.Repositories.Domain.Repositories.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Redis.Om.Repositories.Templates.RedisOmDocument", Version = "1.0")]

namespace Redis.Om.Repositories.Infrastructure.Persistence.Documents
{
    internal class InvoiceLogoDocument : IInvoiceLogoDocument
    {
        public string Url { get; set; } = default!;

        public InvoiceLogo ToEntity(InvoiceLogo? entity = default)
        {
            entity ??= new InvoiceLogo();

            entity.Url = Url ?? throw new Exception($"{nameof(entity.Url)} is null");

            return entity;
        }

        public InvoiceLogoDocument PopulateFromEntity(InvoiceLogo entity)
        {
            Url = entity.Url;

            return this;
        }

        public static InvoiceLogoDocument? FromEntity(InvoiceLogo? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new InvoiceLogoDocument().PopulateFromEntity(entity);
        }
    }
}