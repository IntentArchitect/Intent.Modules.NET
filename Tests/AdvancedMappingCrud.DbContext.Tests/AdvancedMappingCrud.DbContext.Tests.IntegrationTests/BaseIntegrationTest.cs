using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.BaseIntegrationTest", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.IntegrationTests
{
    public class BaseIntegrationTest
    {
        public BaseIntegrationTest(IntegrationTestWebAppFactory webAppFactory)
        {
            WebAppFactory = webAppFactory;
        }

        public IntegrationTestWebAppFactory WebAppFactory { get; }

        protected HttpClient CreateClient()
        {
            return WebAppFactory.CreateClient();
        }
    }
}