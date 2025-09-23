using Amazon.DynamoDBv2.DataModel;
using AwsLambdaFunction.Domain.Entities;
using AwsLambdaFunction.Domain.Repositories;
using AwsLambdaFunction.Infrastructure.Persistence;
using AwsLambdaFunction.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBRepository", Version = "1.0")]

namespace AwsLambdaFunction.Infrastructure.Repositories
{
    internal class DynAffiliateDynamoDBRepository : DynamoDBRepositoryBase<DynAffiliate, DynAffiliateDocument, string, object>, IDynAffiliateRepository
    {
        public DynAffiliateDynamoDBRepository(IDynamoDBContext context, DynamoDBUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
        }
    }
}