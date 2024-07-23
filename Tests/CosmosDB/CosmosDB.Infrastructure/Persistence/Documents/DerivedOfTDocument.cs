using System;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class DerivedOfTDocument : BaseOfTDocument<int>, IDerivedOfTDocument, ICosmosDBDocument<DerivedOfT, DerivedOfTDocument>
    {
        public string DerivedAttribute { get; set; } = default!;

        public DerivedOfT ToEntity(DerivedOfT? entity = default)
        {
            entity ??= new DerivedOfT();

            entity.DerivedAttribute = DerivedAttribute ?? throw new Exception($"{nameof(entity.DerivedAttribute)} is null");
            base.ToEntity(entity);

            return entity;
        }

        public DerivedOfTDocument PopulateFromEntity(DerivedOfT entity, Func<string, string?> getEtag)
        {
            DerivedAttribute = entity.DerivedAttribute;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;
            base.PopulateFromEntity(entity, getEtag);

            return this;
        }

        public static DerivedOfTDocument? FromEntity(DerivedOfT? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new DerivedOfTDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}