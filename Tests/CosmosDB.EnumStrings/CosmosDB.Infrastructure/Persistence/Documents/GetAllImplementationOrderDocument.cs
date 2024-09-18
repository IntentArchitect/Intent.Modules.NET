using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class GetAllImplementationOrderDocument : IGetAllImplementationOrderDocument
    {
        public GetAllImplementationOrder ToEntity(GetAllImplementationOrder? entity = default)
        {
            entity ??= new GetAllImplementationOrder();

            return entity;
        }

        public GetAllImplementationOrderDocument PopulateFromEntity(GetAllImplementationOrder entity)
        {
            return this;
        }

        public static GetAllImplementationOrderDocument? FromEntity(GetAllImplementationOrder? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new GetAllImplementationOrderDocument().PopulateFromEntity(entity);
        }
    }
}