using System.Threading.Tasks;
using HotChocolate.AzureFunctions;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.HotChocolate.GraphQL.AzureFunctions.GraphQLFunctionTemplate", Version = "1.0")]

namespace GraphQL.AzureFunction.TestApplication.Api
{
    public class GraphQLFunction
    {
        private readonly IGraphQLRequestExecutor _executor;

        public GraphQLFunction(IGraphQLRequestExecutor executor)
        {
            _executor = executor;
        }

        [FunctionName("GraphQLHttpFunction")]
        public Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "graphql/{**slug}")] HttpRequest request)
        {
            return _executor.ExecuteAsync(request);
        }
    }
}