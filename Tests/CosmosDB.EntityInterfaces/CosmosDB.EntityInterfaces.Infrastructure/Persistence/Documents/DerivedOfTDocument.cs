using System;
using CosmosDB.EntityInterfaces.Domain.Entities;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Persistence.Documents
{
    internal class DerivedOfTDocument : BaseOfTDocument<int>, IDerivedOfTDocument, ICosmosDBDocument<IDerivedOfT, DerivedOfT, DerivedOfTDocument>
    {
        public string DerivedAttribute { get; set; } = default!;

        public DerivedOfT ToEntity(DerivedOfT? entity = default)
        {
            entity ??= new DerivedOfT();

            entity.DerivedAttribute = DerivedAttribute ?? throw new Exception($"{nameof(entity.DerivedAttribute)} is null");
            base.ToEntity(entity);

            return entity;
        }

        public DerivedOfTDocument PopulateFromEntity(IDerivedOfT entity, Func<string, string?> getEtag)
        {
            DerivedAttribute = entity.DerivedAttribute;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;
            base.PopulateFromEntity(entity, getEtag);

            return this;
        }

        public static DerivedOfTDocument? FromEntity(IDerivedOfT? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new DerivedOfTDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}