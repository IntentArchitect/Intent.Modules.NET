using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocumentOfTInterface", Version = "1.0")]

namespace AwsLambdaFunction.Infrastructure.Persistence.Documents
{
    internal interface IDynamoDBDocument<TDomain, out TDocument>
        where TDomain : class
        where TDocument : IDynamoDBDocument<TDomain, TDocument>
    {
        object GetKey();
        int? GetVersion();
        TDocument PopulateFromEntity(TDomain entity, Func<object, int?> getVersion);
        TDomain ToEntity(TDomain? entity = null);
    }
}