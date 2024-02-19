using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class GetAllImplementation.OrderDocument : IGetAllImplementationOrderDocument
    {
        public GetAllImplementationOrder ToEntity(GetAllImplementationOrder? entity = default)
    {
        entity ??= new GetAllImplementationOrder();

        return entity;
    }

    public GetAllImplementation.OrderDocument PopulateFromEntity(GetAllImplementationOrder entity)
    {
        return this;
    }

    public static GetAllImplementation.OrderDocument? FromEntity(GetAllImplementationOrder? entity)
    {
        if (entity is null)
        {
            return null;
        }

        return new GetAllImplementation.OrderDocument().PopulateFromEntity(entity);
    }
}
}