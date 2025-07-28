using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocumentOfTInterface", Version = "1.0")]

namespace DynamoDbTests.EntityInterfaces.Infrastructure.Persistence.Documents
{
    internal interface IDynamoDBDocument<in TDomain, TDomainState, out TDocument>
        where TDomain : class
        where TDomainState : class, TDomain
        where TDocument : IDynamoDBDocument<TDomain, TDomainState, TDocument>
    {
        object GetKey();
        int? GetVersion();
        TDocument PopulateFromEntity(TDomain entity, Func<object, int?> getVersion);
        TDomainState ToEntity(TDomainState? entity = null);
    }
}